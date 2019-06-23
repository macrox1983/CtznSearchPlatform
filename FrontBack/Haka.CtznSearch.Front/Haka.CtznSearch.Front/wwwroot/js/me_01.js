"use strict";
var me = new (function ()
{
    this.EventNodeNew = 'EventNodeNew';
    this.EventSelectedChanged = 'EventSelectedChanged';
    this.EventYouAreHereChanged = 'EventYouAreHereChanged';

    this.Mode_View = "Mode_View";

    this.Mode_SetPoint = "Mode_SetPoint";
    this.Mode_Use = "Mode_Use";        

    this.Mode_MapEdit = "Mode_MapEdit";
    this.Tool_CreateNodes = "Tool_CreateNodes"; 
    this.Tool_ReplaceNodes = "Tool_ReplaceNodes";
    this.Tool_SelectNodes = "Tool_SelectNodes";
    this.Tool_SelectWays = "Tool_SelectWays";
    this.Tool_SelectAllUnderPoint = "Tool_SelectAllUnderPoint";

    this.MapEditor = function (mapElementId)
    {
        var _instance = this;
        var _env = new meLib.MapEnvironment();
        var _sources = {};
        var _fds = [];
        var _mapObjects = { nodes: [], ways: [], relations: [] };        
        var _ids = { node: -1, way: -1, relation: -1 };
        var _stackSelected = [];            
        var _searchPoints = [];

        let config = {
            mapElementId: mapElementId,
            sources: _sources,
            env: _env,
            fds: _fds,
            handlePointerMoveEvent: handlePointerMoveEvent,
            handlePointerDownEvent: handlePointerDownEvent,
            handlePointerUpEvent: handlePointerUpEvent,
            handlePointerDragEvent: handlePointerDragEvent,
            handleMapZoomChangeEvent: handleMapZoomChangeEvent
        };
        var _mapController = new meLib.MapController(config);
        var _netController = new meLib.NetController(this, _env);
        var _map = _mapController.getMap();
        var _eventPointerData = new meLib.PointsEventsData(_map);
        document.addEventListener("keydown", handleKeyDownEvent, false);
        document.addEventListener("keyup", handleKeyUpEvent, false);

        this.addEventListener = function (event, handler) { _map.getTargetElement().addEventListener(event, handler); }
        this.removeEventListener = function (event, handler) { _map.getTargetElement().removeEventListener(event, handler); }
        /*
        this.test = () =>
        {
            _mapController.test();   
            let coord = osm.tile2Coord(4,4, 3);
            let c = [coord[0], coord[1]];
            _map.getView().setCenter(osm.transformCoordinatesForOL(c));
            _map.getView().setZoom(3);
        };*/


        this.changeTag = function (item, tag, value)
        {
            item.changeTag(tag, value);
            dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
        }     
        this.createZone = function (shape)
        {
            let node = new osm.Node(_ids.node, coords[0], coords[1]);
            _ids.node--;
            let fd = createFeature(node, _sources['vmap']);
            _mapObjects.nodes.push(node);
            node.changeTag('gs_lvl', _env.level);
            node.changeTag('gs_gp', '1');
            dispatchEvent(new CustomEvent(me.EventNodeNew, { detail: { fId: fd.id, node: node, nodeAction: "create" } }));
            return node;
        }
        this.createNode = function (coords)
        {
            let node = new osm.Node(_ids.node, coords[0], coords[1]);
            _ids.node--;
            let fd = createFeature(node, _sources['vmap']);
            _mapObjects.nodes.push(node);
           // node.changeTag('gs_lvl', _env.level);
            node.changeTag('gs_gp', '1');
            dispatchEvent(new CustomEvent(me.EventNodeNew, { detail: { fId: fd.id, node: node, nodeAction: "create" } }));
            return node;
        }
        this.createWayByNodes = function (nodes, way)
        {
            _stackSelected.clear();
            _stackSelected.addRange(nodes);
            if (way) _stackSelected.push(way);
            return this.createWay();
        }
        this.createWay = function ()
        {
            let ways = _stackSelected.where((w) => { return w.getType() === osm.Type_Way; });
            if (ways.length > 1) { gs.showMessage('No more 1 way can be selected to change!'); return; }
            let way = ways.first();
            let nodes = _stackSelected.where((w) => { return w.getType() === osm.Type_Node; });
            if (nodes.length < 2) { gs.showMessage('No less 2 nodes must be selected!'); return; }
            if (!way) {
                way = new osm.Way(_ids.way);
                _ids.way--;                
                nodes.forEach(node => { way.addNode(node); });
                let fd = createFeature(way, _sources['vmap']);
                _mapObjects.ways.push(way);
                way.changeTag('gs_lvl', _env.level);
                way.changeTag('gs_gp', '1');
            } else {                
                way.nds.slice().forEach(nd => { way.removeNode(nd); });
                nodes.forEach(node => { way.addNode(node); });
            }
            this.unselectAll();
            way.setSelected(true);            
            _stackSelected.push(way);
            dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
            return way;
        }        

        this.createMapIcon = function (coord, icon)
        {
            return createFeature(new meLib.MapIcon(coord, icon), _sources['mapIcons']);
        }

        this.createTradeRoomLine = function (count)
        {
            let nodes = _stackSelected.where((w) => { return w.getType() === osm.Type_Node; });
            if (nodes.length !== 4) { gs.showMessage('4 nodes must be selected!'); return; }
            let size = (1 / count);
            let fdx = (nodes[3].lon - nodes[0].lon) * size; let fdy = (nodes[3].lat - nodes[0].lat) * size;
            let bdx = (nodes[2].lon - nodes[1].lon) * size; let bdy = (nodes[2].lat - nodes[1].lat) * size;                        
            let fnd = []; fnd[0] = nodes[0]; fnd[count] = nodes[3];
            let bnd = []; bnd[0] = nodes[1]; bnd[count] = nodes[2];
            for (let i = 1; i < count; i++) {
                fnd[i] = this.createNode([nodes[0].lon + fdx * i, nodes[0].lat + fdy * i]);
                bnd[i] = this.createNode([nodes[1].lon + bdx * i, nodes[1].lat + bdy * i]);
            }
            let ways = [];
            this.unselectAll();
            for (let i = 0; i < count; i++) {
                this.unselectAll();
                _stackSelected.push(fnd[i]);
                _stackSelected.push(bnd[i]);
                _stackSelected.push(bnd[i + 1]);
                _stackSelected.push(fnd[i + 1]);
                let way = this.createWay();
                way.changeTag('gs_build', 'traderoom');
                ways.push(way);
            }
            this.unselectAll();
            _stackSelected.addRange(ways);
            ways.forEach(w => { w.setSelected(true); });
            dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
        }

        this.setRangeNumbersToSelected = function (from)
        {          
            let ways = _stackSelected.where((w) => { return w.getType() === osm.Type_Way; });
            let step = from;
            let nds = [];
            ways.forEach(way =>
            {
                let node = this.createNode(way.getCenter());
                node.changeTag('gs_label_number', step+'');
                step++;
                nds.push(node);
            });
            this.unselectAll();
            _stackSelected.addRange(nds);
            nds.forEach(nd => { nd.setSelected(true); });
            dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
        }

        this.unselectAll = function ()
        {
            _stackSelected.forEach(item => { item.setSelected(false); });
            _stackSelected.clear();
            dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
            if (_env.searchPointLockFd) _env.searchPointLockFd.item.setCoordinate([0, 0]);
        }

        this.getLevel = function () { return _env.level; }
        this.setLevel = function (newLevel)
        {
            if (newLevel < 0) return;
            if (_env.limits.maxLevel < newLevel) return;
            _env.level = newLevel;
            _netController.getCurrentElements(_env);            
        }

        

        this.setMapGroupId = function (mapGroupId)
        {
            _env.groupId = mapGroupId;
            _netController.getMapGroup(setMapGroup);
        }
       
        
        this.setMode = function (mode) { _env.mode = mode; _fds.forEach(fd => { fd.refresh(); }); };
        this.setTool = function (tool) { _env.tool = tool; _fds.forEach(fd => { fd.refresh(); }); };
        this.fill = function (changeSet)
        {
            _fds.forEach(fd => { if (fd) destroyFeature(fd.id); });
            _fds.clear();
            _mapObjects = { nodes: [], ways: [], relations: [] };
            _ids = { node: -1, way: -1, relation: -1 };            
            changeSet.nodes.forEach(nodeData =>
            {
                let node = new osm.Node(); node.fillFromObj(nodeData);
                let fd = createFeature(node, _sources['vmap']);
                _mapObjects.nodes.push(node);
            });
            changeSet.ways.forEach(wayData =>
            {
                let way = new osm.Way(); way.fillFromObj(wayData, _mapObjects.nodes);                
                let fd = createFeature(way, _sources['vmap']);
                _mapObjects.ways.push(way);
            });
            _stackSelected.clear();
            this.unselectAll();

            _env.youAreHereFd = createFeature(new meLib.MapIcon(_env.youAreHereCoord, meLib.MapIconType_YouAreHere), _sources['mapIcons']);
            _env.searchPointLockFd = createFeature(new meLib.MapIcon([0, 0], meLib.MapIconType_SearchPointLock), _sources['mapIcons']);

            _searchPoints.slice().forEach(fd =>
            {
                let point = fd.item.getPoint();
                _instance.addPoint(point);
            })
        }
        this.pushChangeSet = function () { _netController.pushChangeSet(); }
        this.getChangeSet = () => 
        {
            let cs = { nodes: [], ways: [], relations: [] };
            _mapObjects.nodes.forEach(node => { if (node.isNew() || node.isChanged()) cs.nodes.push(node); })
            _mapObjects.ways.forEach(way =>
            {
                if (way.isNew() || way.isChanged())
                    cs.ways.push(way);
            })
            _mapObjects.relations.forEach(relation => { if (relation.isNew() || relation.isChanged()) cs.relations.push(relation); })
            return cs;
        }

        this.getYouAreHere = () => { return _env.youAreHereCoord.slice(); }
        this.setYouAreHere = function (coord)
        {
            _env.youAreHereCoord = coord;
            if (_env.youAreHereFd) _env.youAreHereFd.item.setCoordinate(coord);
            dispatchEvent(new CustomEvent(me.EventYouAreHereChanged, { detail: coord }));
        }        
        this.getZoom = () => { return _map.getView().getZoom(); }
        this.setZoom = (zoom) => { _map.getView().setZoom(zoom); }
        this.getCenter = () => { return _map.getView().getCenter(); }
        this.setCenter = (coord) => { _map.getView().setCenter(osm.transformCoordinatesForOL(coord)); }
        this.setHeight = function (heigth) { _map.getTargetElement().style.height = heigth; _map.updateSize(); }

        this.addPoint = function (point)
        {
            if (_searchPoints[point.Id]) this.removePoint(point.Id);            
            _searchPoints[point.Id] = createFeature(new meLib.SeachPoint(point), _sources['searchPoints']);            
        };
        this.removePoint = function (pointId)
        {
            let fd = _searchPoints[pointId];
            if (!fd) return;
            destroyFeature(fd.id);
            _searchPoints[pointId] = undefined;
        };
        this.getSearchPoints = () => { let result = []; _searchPoints.forEach(p => { if (!p) return; result.push(p.item); }); return result; }
        this.getSearchPoint = (pointId) => { if (!_searchPoints[pointId]) return undefined; return _searchPoints[pointId].item; }
        this.selectSearchPoint = (pointId) =>
        {
            _instance.unselectAll();
            let item = this.getSearchPoint(pointId);
            if (!item) return;
            _stackSelected.push(item);
            _env.searchPointLockFd.item.setCoordinate(item.coordinateGS());
            dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
        }

        function setMapGroup(mapGroup)
        {
            _env.groupId = mapGroup.id;            
         
            _env.limits.topLeft = [mapGroup.topLeftLon, mapGroup.topLeftLat];
            _env.limits.bottomRight = [mapGroup.bottomRightLon, mapGroup.bottomRightLat];
            _env.limits.maxLevel = mapGroup.maxLevel;
            _env.limits.maxZoom = mapGroup.maxZoom;
            _env.limits.minZoom = mapGroup.minZoom;
            _mapController.setLimits();

            let view = _map.getView();
            let existCenter = osm.transformCoordinatesForGS(view.getCenter());
            let existZoom = _instance.getZoom();
            let needChangeCenter = true;
            if ((existCenter[0] < _env.limits.topLeft[0]) || (existCenter[1] > _env.limits.topLeft[1]) || (existCenter[0] > _env.limits.bottomRight[0]) || (existCenter[1] < _env.limits.bottomRight[1])) {
                let newCenter = osm.transformCoordinatesForOL(osm.getCenter([_env.limits.topLeft, _env.limits.bottomRight]));
                view.setCenter(newCenter);
                view.setZoom(mapGroup.minZoom);
            } else {
                if ((existZoom < mapGroup.minZoom) || (existZoom > mapGroup.maxZoom)) view.setZoom(mapGroup.minZoom);
            }
        }


        function createFeature(item, source)
        {
            let fId = _fds.length;            
            _fds[fId] = new meLib.FeatureDescription(fId, item, source, _env);
            return _fds[fId];
        }
        function destroyFeature(fId)
        {
            let fd = _fds[fId];
            if (!fd) return;
            fd.source.removeFeature(fd.feature);
            _fds[fId] = undefined;
        }
        function dispatchEvent(event)
        {
            _mapController.getHtmlElement().dispatchEvent(event);
        }     




        function handleKeyDownEvent(evt)
        {
            var keyCode = evt.keyCode;            
            if (keyCode === 17) _eventPointerData.ctrPressed = true; //ctr
        }
        function handleKeyUpEvent(evt)
        {
            var keyCode = evt.keyCode;
            if (keyCode === 27) _instance.unselectAll(); //esc
            if (keyCode === 17) _eventPointerData.ctrPressed = false; //ctr
        }
        
        function handleMapZoomChangeEvent(zoom)
        {
            _env.zoom = zoom;
            _fds.forEach(fd => { if (!fd) return; fd.refresh(); });          
        };
        function handlePointerMoveEvent(evt)
        {        
            var element = _map.getTargetElement();
            _eventPointerData.fdsUnder.clear();
            if (_env.mode === me.Mode_View) return;
            if (_env.mode === me.Mode_Use) {
                _map.forEachFeatureAtPixel(evt.pixel, function (feature)
                {
                    let fid = feature.getId();
                    if (!fid) {
                        let cluster = feature.get("features");
                        cluster.forEach(fc => { let desc = _fds[fc.getId()]; if (desc.item.getType() === meLib.Type_SearchPoint) _eventPointerData.fdsUnder.push(desc); });
                    } else {
                        let desc = _fds[feature.getId()];
                        let t = desc.item.getType();
                        if ((t === meLib.Type_MapIcon) || (t === meLib.Type_SearchPoint)) _eventPointerData.fdsUnder.push(desc);
                        if (t === osm.Type_Way) {
                            let tags = desc.item.getTagsDict();
                            if (tags.gs_build === 'traderoom') _eventPointerData.fdsUnder.push(desc);
                        }                    
                    }
                });
            }
            if (_env.mode === me.Mode_SetPoint) {
                _map.forEachFeatureAtPixel(evt.pixel, function (feature)
                {
                    let desc = _fds[feature.getId()];
                    if (!desc.item.getTagsDict) return;
                    let tags = desc.item.getTagsDict();
                    if (!tags.gs_build) return;
                    if (tags.gs_build !== 'traderoom') return;
                    _eventPointerData.fdsUnder.push(desc);
                });
            }
            if (_env.mode === me.Mode_MapEdit) {
                switch (_env.tool) {                    
                    case me.Tool_CreateNodes:                    
                        _eventPointerData.setCursorPointer(true);
                        return;
                    case me.Tool_SelectNodes:
                    case me.Tool_ReplaceNodes:
                        _map.forEachFeatureAtPixel(evt.pixel, function (feature) { let desc = _fds[feature.getId()]; if (desc.item.getType() === osm.Type_Node) _eventPointerData.fdsUnder.push(desc); });
                        break;
                    case me.Tool_SelectWays:
                        _map.forEachFeatureAtPixel(evt.pixel, function (feature) { let desc = _fds[feature.getId()]; if (desc.item.getType() === osm.Type_Way) _eventPointerData.fdsUnder.push(desc); });
                        break;
                    case me.Tool_SelectAllUnderPoint:
                        _map.forEachFeatureAtPixel(evt.pixel, function (feature) { let desc = _fds[feature.getId()]; _eventPointerData.fdsUnder.push(desc); });
                        break;                             
                }
            }
            if (_eventPointerData.fdsUnder.length > 0) _eventPointerData.setCursorPointer(true); else _eventPointerData.setCursorPointer(false);            
        };
        function handlePointerDownEvent (evt)
        {
            _eventPointerData.lastDownPixel = evt.pixel;
            let fds = [];
            if (_env.tool === me.Tool_ReplaceNodes) { 
                _map.forEachFeatureAtPixel(evt.pixel, function (feature) { let desc = _fds[feature.getId()]; if (desc.item.getType() === osm.Type_Node) fds.push(desc); });
                _eventPointerData.selectedFd = fds.first();
                if (_eventPointerData.selectedFd) { _eventPointerData.coordinate = evt.coordinate; }
            } else
            if (_env.mode === me.Mode_Use) {
                _map.forEachFeatureAtPixel(evt.pixel, function (feature)
                {
                    let fId = feature.getId();
                    if (!fId) return;
                    let desc = _fds[feature.getId()];
                    if (desc.item.getType() === meLib.Type_MapIcon) if (desc.item.getIconType() === meLib.MapIconType_YouAreHere) fds.push(desc);
                });
                _eventPointerData.selectedFd = fds.first();
                if (_eventPointerData.selectedFd) { _eventPointerData.coordinate = evt.coordinate; }
            } else return;
            return !_eventPointerData.selectedFd;
        };
        function handlePointerDragEvent (evt)
        {
            if (!_eventPointerData.selectedFd) return;
            let fd = _eventPointerData.selectedFd;
            let item = fd.item;
            _eventPointerData.changedPosition = true;
            var deltaX = evt.coordinate[0] - _eventPointerData.coordinate[0];
            var deltaY = evt.coordinate[1] - _eventPointerData.coordinate[1];

            var geometry = fd.feature.getGeometry();
            geometry.translate(deltaX, deltaY);
            _eventPointerData.coordinate = evt.coordinate.slice();

            let clearCoordinate = osm.transformCoordinatesForGS(geometry.getCoordinates());
            if ((item.lon !== clearCoordinate[0]) || (item.lat !== clearCoordinate[1])) {
                item.setCoordinate(clearCoordinate);
                if (item.getType() === osm.Type_Node) 
                    if (!item.isNew()) item.setChanged(true);                
            }
            fd.refresh();
        };

        function handlePointerUpEvent (evt)
        {    
            let node;
            let clearCoordinate = osm.transformCoordinatesForGS(evt.coordinate);

            if (_env.mode === me.Mode_View) { }
            if (_env.mode === me.Mode_Use) {
                if (_eventPointerData.lastClickPixel[0] === evt.pixel[0]) _instance.setYouAreHere(clearCoordinate);                
                if (_eventPointerData.selectedFd) _instance.setYouAreHere(_eventPointerData.selectedFd.item.coordinateGS());                
                if (_eventPointerData.lastDownPixel[0] === evt.pixel[0]) 
                    if (_eventPointerData.fdsUnder.length > 0) {
                        let firstFd = _eventPointerData.fdsUnder.first();
                        let item = firstFd.item;
                        let itemType = item.getType();
                        switch (itemType) {
                            case meLib.Type_SearchPoint:
                                _instance.unselectAll();
                                _stackSelected.push(item);
                                _env.searchPointLockFd.item.setCoordinate(item.coordinateGS());
                                dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
                                break;
                            case osm.Type_Way:
                                _instance.unselectAll();
                                item.setSelected(true);
                                if (item.isSelected()) _stackSelected.push(item); else _stackSelected.remove(item);
                                dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
                                break;
                        }
                    }                                    
            }
            if (_env.mode === me.Mode_SetPoint) {
                if (_eventPointerData.lastClickPixel[0] === evt.pixel[0]) _instance.setYouAreHere(clearCoordinate);
                if (_eventPointerData.lastDownPixel[0] === evt.pixel[0]) {                    
                    if (_eventPointerData.fdsUnder.length > 0) {                        
                        let firstFd = _eventPointerData.fdsUnder.first();
                        let item = firstFd.item;
                        item.setSelected(!item.isSelected());
                        if (item.isSelected()) _stackSelected.push(item); else _stackSelected.remove(item);
                        dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));                        
                    }                    
                }
            }
           
            if (_env.mode === me.Mode_MapEdit) {
                switch (_env.tool) {                    
                    case me.Tool_CreateNodes:
                        if (_eventPointerData.lastDownPixel[0] !== evt.pixel[0]) break;
                        clearCoordinate = osm.transformCoordinatesForGS(evt.coordinate);
                        _instance.createNode(clearCoordinate);
                        break;                
                    case me.Tool_ReplaceNodes:
                        if (!_eventPointerData.selectedFd) break;
                        if (!_eventPointerData.changedPosition) break;
                        node = _eventPointerData.selectedFd.item;
                        clearCoordinate = osm.transformCoordinatesForGS(_eventPointerData.selectedFd.feature.getGeometry().getCoordinates());
                        node.setCoordinate(clearCoordinate);
                        if (!node.isNew()) node.setChanged(true);
                        break;
                    case me.Tool_SelectNodes:
                    case me.Tool_SelectWays:                    
                        if (_eventPointerData.lastDownPixel[0] !== evt.pixel[0]) break;
                        if (_eventPointerData.fdsUnder.length === 0) break;
                        if (_eventPointerData.ctrPressed) _instance.unselectAll();
                        let firstFd = _eventPointerData.fdsUnder.first();
                        let item = firstFd.item;
                        item.setSelected(!item.isSelected());
                        if (item.isSelected()) _stackSelected.push(item); else _stackSelected.remove(item);
                        dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
                        firstFd.refresh();
                        break;
                    case me.Tool_SelectAllUnderPoint:
                        if (_eventPointerData.lastDownPixel[0] !== evt.pixel[0]) break;
                        if (_eventPointerData.fdsUnder.length === 0) break;
                        if (_eventPointerData.ctrPressed) _instance.unselectAll();
                        let itemsToAdd = _eventPointerData.fdsUnder.select((i) => { return i.item; }).where((i) => { return _stackSelected.indexOf(i) < 0; });
                        _stackSelected.addRange(itemsToAdd);
                        _eventPointerData.fdsUnder.forEach(fd => { fd.item.setSelected(true); });
                        dispatchEvent(new CustomEvent(me.EventSelectedChanged, { detail: _stackSelected.slice() }));
                        break;
                }
            }
            _eventPointerData.changedPosition = false;
            _eventPointerData.selectedFd = undefined;          
            _eventPointerData.lastClickPixel = evt.pixel;
        };
    };   

 
})();

