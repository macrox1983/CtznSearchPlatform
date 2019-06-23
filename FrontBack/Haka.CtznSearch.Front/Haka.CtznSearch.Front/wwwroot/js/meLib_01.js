"use strict";
var meLib = new (function ()
{
    this.Type_MapIcon = "Type_MapIcon";
    this.Type_SearchPoint = "Type_SearchPoint";

    this.MapIconType_YouAreHere = "youAreHere";
    this.MapIconType_SearchPointLock = "searchPointLock";
    this.MapIconType_Victim = "victim";
    this.MapIconType_Hunter = "hunter";
    this.MapIconType_CameraMatch = "cameraMatch";

    let _mapIconStyles = {};
    _mapIconStyles[this.MapIconType_YouAreHere] = new ol.style.Style({ image: new ol.style.Icon({ anchor: [0.5, 1], scale: 0.25,  anchorXUnits: 'fraction', anchorYUnits: 'fraction', opacity: 1, src: '/images/here.svg' }), zIndex: 4 });
    _mapIconStyles[this.MapIconType_SearchPointLock] = new ol.style.Style({ image: new ol.style.Icon({ anchor: [0.5, 14], anchorXUnits: 'fraction', anchorYUnits: 'pixels', opacity: 1, src: '/images/lock.png' }), zIndex: 1 });
    _mapIconStyles[this.MapIconType_Victim] = new ol.style.Style({ image: new ol.style.Icon({ anchor: [0.5, 1], scale: 0.25, anchorXUnits: 'fraction', anchorYUnits: 'fraction', opacity: 1, src: '/images/victim.svg' }), zIndex: 3 });
    _mapIconStyles[this.MapIconType_Hunter] = new ol.style.Style({ image: new ol.style.Icon({ anchor: [0.5, 1], scale: 0.25, anchorXUnits: 'fraction', anchorYUnits: 'fraction', opacity: 1, src: '/images/hunter.svg' }), zIndex: 2 });
    _mapIconStyles[this.MapIconType_CameraMatch] = new ol.style.Style({ image: new ol.style.Icon({ anchor: [0.5, 1], scale: 0.25, anchorXUnits: 'fraction', anchorYUnits: 'fraction', opacity: 1, src: '/images/cameraMatch.svg' }), zIndex: 3 });
    
    this.MapEnvironment = function ()
    {
        this.zoom = 0;
        this.mode = me.Mode_View;
        this.tool = '';
        this.groupId = 1;
        this.level = 1;
        this.limits = {};

        this.youAreHereCoord = [0, 0];
        this.youAreHereFd;
        this.searchPointLockFd;
    };

    this.PointsEventsData = function (map)
    {
        let _map = map;
        this.lastClickPixel = [0, 0];
        this.lastDownPixel = [0, 0];
        this.coordinate = null;
        this.cursor = 'pointer';
        this.selectedFd = undefined;
        this.fdsUnder = [];
        this.changedPosition = false;
        this.previousCursor = undefined;
        this.ctrPressed = false;

        this.setCursorPointer = (value) =>
        {
            let element = _map.getTargetElement();
            if (value) {
                if (element.style.cursor !== this.cursor) {
                    this.previousCursor = element.style.cursor;
                    element.style.cursor = this.cursor;
                }
            } else {
                if (this.previousCursor !== undefined) {
                    element.style.cursor = this.previousCursor;
                    this._previousCursor = undefined;
                }
            }
        }
    };

    this.FeatureDescription = function (fdId, item, source, env)
    {
        let _env = env;
        this.id = fdId;
        this.item = item;
        this.source = source;
        this.feature = new ol.Feature();    
        this.feature.setId(this.id);

        this.refresh = () =>
        {
            switch (item.getType()) {
                case osm.Type_Node:
                    this.feature.setGeometry(new ol.geom.Point(item.coordinateOL()));
                    break;
                case osm.Type_Way:
                    if ((item.tags.where((p) => { return p.k === "gs_build"; }).length > 0) || (item.tags.where((p) => { return p.k === "gs_way"; }).length > 0)) {
                        this.feature.setGeometry(new ol.geom.Polygon([item.coordinatesOL()]));    
                    } else
                    this.feature.setGeometry(new ol.geom.LineString(item.coordinatesOL()));
                    break;
                case meLib.Type_MapIcon:
                    this.feature.setGeometry(new ol.geom.Point(item.coordinateOL()));
                    this.feature.setStyle(item.getOLStyle(_env));            
                    return;
                default:
                    this.feature.setGeometry(new ol.geom.Point(item.coordinateOL()));
                    return;
            }
            this.feature.setStyle(item.getOLStyle(_env));            
            this.item.getLinkedMapElement().forEach(i =>
            {
                if (!i) return;
                let f = i.getFd();
                f.refresh();
            });
        }

        item.setFd(this);
        this.refresh();
        source.addFeature(this.feature); 
    }

    this.SeachPoint = function (point)
    {
        let _point = point;

        this.getPoint = () => { return _point; }
        this.getType = () => { return meLib.Type_SearchPoint; }
        this.coordinateGS = () => { return [parseFloat(_point.Options.Longitude), parseFloat(_point.Options.Latitude)]; }
        this.coordinateOL = () => { return osm.transformCoordinatesForOL(this.coordinateGS()); }

        var _fd = undefined;
        this.getFd = () => { return _fd; }
        this.setFd = (value) => { _fd = value; }

        var _selected = false;
        this.getSelected = () => { return _selected; }
        this.setSelected = (value) => { _selected = value;}
    }

    this.MapIcon = function (coord, iconType)
    {
        let _coord = coord;
        let _iconType = iconType;

        this.getIconType = () => { return _iconType; }

        this.getType = () => { return meLib.Type_MapIcon; }
        this.coordinateGS = () => { return _coord; }
        this.coordinateOL = () => { return osm.transformCoordinatesForOL(_coord); }

        var _fd = undefined;
        this.getFd = () => { return _fd; }
        this.setFd = (value) => { _fd = value; }

        this.setCoordinate = (coord) => { _coord = coord; _fd.refresh(); }

        this.getOLStyle = function (env) { return _mapIconStyles[_iconType]; }
    }
    
    this.MapController = function (config)
    {
        let _config = config;
        var _elMap = gs.getById(config.mapElementId);

        var _layers = {};
        var _sources = config.sources;
        var _env = config.env;
        var _fds = config.fds;

        _sources['vmap'] = new ol.source.Vector({ features: [] });            
        _sources['mapIcons'] = new ol.source.Vector({ features: [] });       
        _sources['searchPoints'] = new ol.source.Vector({ features: [] });       
        _sources['searchClusters'] = new ol.source.Cluster({ distance: 30, source: _sources['searchPoints'] });
        

        _layers['osm'] = new ol.layer.Tile({ source: new ol.source.OSM({ crossOrigin: null})});
        _layers['tdebug'] = new ol.layer.Tile({ source: new ol.source.TileDebug({ projection: 'EPSG:3857', tileGrid: new ol.source.OSM().getTileGrid() }) });        
        _layers['vmap'] = new ol.layer.Vector({ source: _sources['vmap'] });                
        _layers['searchClusters'] = new ol.layer.Vector({ source: _sources['searchClusters'], style: handlerStyleClusters });         
        _layers['mapIcons'] = new ol.layer.Vector({ source: _sources['mapIcons'] });                

        let initZoom = 1;
        let initCoordinate = [0, 0];
        let actions = { doubleClickZoom: false, altShiftDragRotate: false, pinchRotate: false };

        let _view = new ol.View({ center: initCoordinate, zoom: initZoom });
        var _map = new ol.Map({
            interactions: ol.interaction.defaults(actions),//.extend([new mapActions.Pointer()]),
            layers: [_layers['osm'],_layers['vmap'], _layers['searchClusters'], _layers['mapIcons'],/*,  /*, _layers['tdebug']*/],
            controls: ol.control.defaults({ attribution: false }).extend([new ol.control.Attribution({ collapsible: false })]),
            target: _elMap.id,
            view: _view
        });
        _layers['osm'].setOpacity(0.5);
        _layers['vmap'].setOpacity(0.4);
        _map.on('pointermove', config.handlePointerMoveEvent);
        _map.on('pointerdown', config.handlePointerDownEvent);
        _map.on('pointerup', config.handlePointerUpEvent);
        _map.on('pointerdrag', config.handlePointerDragEvent);
        _map.on('moveend', handleOnChangeEvent);
        _map.on('movestart', handleOnChangeEvent);

        

        /*
        this.test = () =>
        {
            _elMap.style.width = '512px';
            _elMap.style.height = '512px';
            _map.updateSize();
        }*/

        this.getMap = () => { return _map; }
        this.getHtmlElement = () => { return _map.getTargetElement(); }
        this.setLimits = () =>
        {
            let wasCenter = _view.getCenter();
            let wasZoom = _view.getZoom();
            let tl = osm.transformCoordinatesForOL(_env.limits.topLeft);
            let br = osm.transformCoordinatesForOL(_env.limits.bottomRight);

            _view = new ol.View({
                center: wasCenter, zoom: wasZoom, 
                extent: [tl[0], br[1], br[0], tl[1]]
            });
            _view.setMaxZoom(_env.limits.maxZoom);
            _view.setMinZoom(_env.limits.minZoom);
            _map.setView(_view);
        }


        let _onChangeEventData = { zoom: initZoom, coordinate: initCoordinate };
        function handleOnChangeEvent(evt)
        {
            let zoom = _view.getZoom();
            if (_onChangeEventData.zoom !== zoom) { _onChangeEventData.zoom = zoom; if (_config.handleMapZoomChangeEvent) return _config.handleMapZoomChangeEvent(zoom); }            
        }
        function handlerStyleClusters(feature)
        {
            var size = feature.get('features').length;
            let imageSize = 10 + (size / _sources['searchPoints'].getFeatures().length) * 10;
            var color = '#3399CC';
            var text = size.toString();
            if (size === 1) {
                let fdId = feature.get('features')[0].getId();
                let fd = _fds[fdId];
                let point = fd.item.getPoint();
                imageSize = 10; color = '#33cc33a8';
                if (point.SearchLevel === 2) color = '#ccc333a8';
                if (point.SearchLevel === 3) color = '#cc4533a8';
                text = "";
            }

            return new ol.style.Style({
                image: new ol.style.Circle({
                    radius: imageSize,
                    stroke: new ol.style.Stroke({ color: '#fff' }),
                    fill: new ol.style.Fill({ color: color })
                }),
                text: new ol.style.Text({
                    text: text, fill: new ol.style.Fill({ color: '#fff' })
                })
            });
        }
    }

    this.NetController = function (mapEditor, env)
    {
        let _instance = this;
        let _mapEditor = mapEditor;
        let _env = env;

        this.pushChangeSet = function ()
        {
            var changeSet = _mapEditor.getChangeSet();
            let data = { 'changeSetJSON': JSON.stringify(changeSet) };
            gs.disable('map');
            gs.Net.post('/maps/AJAXEditor/pushChangeSet', data, function (result)
            {
                gs.enable('map');
                switch (result.error) {
                    case 0: _instance.getCurrentElements(); return;
                    default: gs.showMessage(gs.Net.getErrorByCode(result) + '<br/>'); break;
                }
            });
        }

        this.getMapGroup = function (callback)
        {
            gs.disable('map');
            gs.Net.get(`/maps/AJAXEditor/getMapGroup?groupId=${_env.groupId}`, function (result)
            {
                gs.enable('map');
                switch (result.error) {
                    case 0:
                        callback(result.object);
                        _instance.getCurrentElements();
                        return;
                    default: gs.showMessage(gs.Net.getErrorByCode(result) + '<br/>'); break;
                }
            });
        }

        this.getCurrentElements = function ()
        {
            gs.disable('map');
            gs.Net.get(`/maps/AJAXEditor/getCurrentElements?groupId=${_env.groupId}&level=${_env.level}`, function (result)
            {
                gs.enable('map');
                switch (result.error) {
                    case 0:
                        _mapEditor.fill(result.object);
                        return;
                    default: gs.showMessage(gs.Net.getErrorByCode(result) + '<br/>'); break;
                }
            });
        }
    }
})();

