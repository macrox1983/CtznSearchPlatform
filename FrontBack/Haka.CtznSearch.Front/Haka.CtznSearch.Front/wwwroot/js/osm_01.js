"use strict";
var osm = new (function ()
{
    this.Type_Node = "node";
    this.Type_Way = "way";
    this.Type_Relation = "relation";        

    this.transformCoordinatesForOL = function (lonlat) { return ol.proj.fromLonLat(lonlat, 'EPSG:3857'); }
    this.transformCoordinatesForGS = function (lonlat) { return ol.proj.toLonLat(lonlat, 'EPSG:3857'); }
    this.getDistance = function (coord1, coord2) { return Math.round(new ol.geom.LineString([osm.transformCoordinatesForOL(coord1), osm.transformCoordinatesForOL(coord2)]).getLength()); }    
    this.getUrlArg = function (coord, zoom) { return `${coord[1].toFixed(6)}_${coord[0].toFixed(6)}_${zoom}`; }    

    this.coord2tile = function (lon, lat, zoom)
    {
        let result = [];        
        result[0] = (Math.floor((lon + 180) / 360 * Math.pow(2, zoom)));
        result[1] = (Math.floor((1 - Math.log(Math.tan(lat * Math.PI / 180) + 1 / Math.cos(lat * Math.PI / 180)) / Math.PI) / 2 * Math.pow(2, zoom)));
        result[2] = zoom;
        return result;
    }
    this.tile2Coord = function(x, y, z)
    {
        let result = [];
        var n = Math.PI - 2 * Math.PI * y / Math.pow(2, z);        
        result[0] = (x / Math.pow(2, z) * 360 - 180);
        result[1] = (180 / Math.PI * Math.atan(0.5 * (Math.exp(n) - Math.exp(-n))));
        result[2] = z;
        return result;
    }
    this.getCenter = (coords) =>
    {        
        let tl = [coords[0][0], coords[0][1]]; let rb = [coords[0][0], coords[0][1]];
        coords.forEach(coord =>
        {
            if (coord[0] < tl[0]) tl[0] = coord[0];
            if (coord[1] < tl[1]) tl[1] = coord[1];
            if (coord[0] > rb[0]) rb[0] = coord[0];
            if (coord[1] > rb[1]) rb[1] = coord[1];
        });
        return [tl[0] + (rb[0] - tl[0]) / 2, tl[1] + (rb[1] - tl[1]) / 2];
    };

    this.Tag = function (k, v)
    {
        this.k = k;
        this.v = v;        
    }

    this.MapElement = function (id, type)
    {
        var _type = type;

        this.id = id;
        this.version = 0;        
        this.changeset = 0;
        this.userId = 0;
        this.visible = true;
        this.timestamp = "2000-01-01T00:00:00Z";
        this.tags = [];     

        this.fillBaseFromObj = function (obj)
        {
            this.id = obj.id;
            this.version = obj.version;
            this.changeset = obj.changeset;
            this.userId = obj.userId;
            this.visible = obj.visible;
            this.timestamp = obj.timestamp;
            this.tags = obj.tags;
        }

        this.getType = () => { return _type; }

        var _changed = false;
        this.getChanged = () => { return _changed; }
        this.setChanged = (value) => { if (_changed === value) return; _changed = value; if (value) this.version++; else this.version--; }

        var _selected = false;
        this.getSelected = () => { return _selected; }
        this.setSelected = (value) => { _selected = value; this.getFd().refresh(); }

        var _fd = undefined;
        this.getFd = () => { return _fd; }
        this.setFd = (value) => { _fd = value; }

        var _linkedMapElements = [];
        this.getLinkedMapElement = () => { return _linkedMapElements.slice(); }
        this.addLinkedMapElement = (item) => { if (_linkedMapElements.indexOf(item) >= 0) return; _linkedMapElements.push(item); }
        this.removeLinkedMapElement = (item) => { _linkedMapElements.remove(item); }

        this.isNew = () => { return this.id < 0; };
        this.isSelected = () => { return this.getSelected(); };
        this.isChanged = () => { return this.getChanged(); };

        this.getTagsDict = () => { let result = {}; this.tags.forEach((i) => { result[i.k] = i.v; }); return result; };
        this.changeTag = function (key, value)
        {
            let tag = this.tags.where((item) => { return (item.k === key); }).first();
            if (tag) {
                if (tag.v === value) return;
                if (value === "") this.tags.remove(tag); else tag.v = value;
            } else {
                tag = new osm.Tag(key, value);
                this.tags.push(tag);
                let tags_ = this.tags;
                if (_tagsExcept[tag.k]) _tagsExcept[tag.k].forEach(tn => { let exist = tags_.where((t) => t.k === tn).first(); if (exist) tags_.remove(exist); });
            }
            this.setChanged(true);
            this.getFd().refresh();
        }
    }

    this.Node = function (id=0, lon=0, lat=0)
    {
        osm.MapElement.call(this, id, osm.Type_Node);
        this.lon = lon;
        this.lat = lat;

        this.setCoordinate = (lonlat) => { this.lon = lonlat[0]; this.lat = lonlat[1]; this.getFd().refresh(); }

        this.coordinateOL = () => { return ol.proj.fromLonLat([this.lon, this.lat], 'EPSG:3857'); }
        this.coordinateGS = () => { return [this.lon, this.lat]; }
        this.toString = () => { return `nd${this.id} ${Math.round(this.lon * 10000000) / 10000000}:${Math.round(this.lat * 10000000) / 10000000}`; }        
        
        this.getOLStyle = function (env)
        {
            switch (env.mode) {                
                case me.Mode_MapEdit:
                    if (this.isSelected()) return _gsStyles.nodeSelected;
                    if (this.isNew()) return _gsStyles.nodeNew;
                    if (this.isChanged()) return _gsStyles.nodeChanged;
                    return _gsStyles.node;
                default:
                    let tobj = this.getTagsDict();
                    if (tobj.gs_min_visible_zoom)
                        if (env.zoom < tobj.gs_min_visible_zoom) return new ol.style.Style();
                    if (tobj.gs_icon) return _gsStyles.gs_icon[tobj.gs_icon];
                    let textRotate = 0;
                    if (tobj.gs_label_rotate)
                        textRotate = parseFloat(tobj.gs_label_rotate) / 180 * 3.14;
                    if (tobj.gs_label_name) return new ol.style.Style({ text: new ol.style.Text({ text: tobj.gs_label_name, rotation: textRotate, fill: new ol.style.Fill({ color: '#404040' }), font: 'bold 11px sans-serif' }), zIndex: 21 });
                    if (tobj.gs_label_number) return new ol.style.Style({ text: new ol.style.Text({ text: tobj.gs_label_number, rotation: textRotate }), zIndex: 21 });
                    return new ol.style.Style();
            }            
        }      
    }
    this.Node.prototype = Object.create(this.MapElement.prototype);    
    this.Node.prototype.fillFromObj = function (obj) { this.fillBaseFromObj(obj); this.lon = obj.lon; this.lat = obj.lat; }    

    this.Way = function (id = 0)
    {
        osm.MapElement.call(this, id, osm.Type_Way);
        this.nds = [];

        this.coordinatesOL = () => { var cs = []; _nodes.forEach(node => { cs.push(node.coordinateOL()); }); return cs; }
        this.coordinatesGS = () => { var cs = []; _nodes.forEach(node => { cs.push(node.coordinateGS()); }); return cs; }
        this.toString = () => { return `way${this.id}`; };
        this.getType = () => { return osm.Type_Way; };

        var _nodes = [];
        var _topleftCoordinate = [-500,-500];

        this.addNode = (node) =>
        {
            if (this.nds.indexOf(node.id) >= 0) { gs.showMessage("Try add node in way more one time!"); return; }
            _nodes.push(node);
            node.addLinkedMapElement(this);
            this.nds.push(node.id);
            this.setChanged(true);
        };     

        this.removeNode = (nodeId) =>
        {
            let node = _nodes.where((n) => { return n.id === nodeId; }).first();
            if (!node) return;
            _nodes.remove(node);
            node.removeLinkedMapElement(this);
            this.nds.remove(nodeId);
            this.setChanged(true);
        };  

        this.getCenter = () =>
        {
            let coords = [];
            _nodes.forEach(node => { coords.push([node.lon, node.lat]); });
            return osm.getCenter(coords);
        };

        this.getOLStyle = function (env)
        {
            let pref = '';
            if (this.isSelected()) pref = 'Selected';
            let gs_build = this.tags.where((item) => { return (item.k === 'gs_build'); }).first();
            if (gs_build) return _gsStyles.gs_build[gs_build.v + pref];            
            let gs_way = this.tags.where((item) => { return (item.k === 'gs_way'); }).first();
            if (gs_way) return _gsStyles.gs_way[gs_way.v + pref];
            let gs_path = this.tags.where((item) => { return (item.k === 'gs_path'); }).first();
            if (gs_path) return _gsStyles.gs_path[gs_path.v + pref];
            if (env.mode === me.Mode_View) return new ol.style.Style();            
            if (this.isNew()) return _gsStyles['wayNew' + pref];
            return _gsStyles['way' + pref];
        }
    }
    this.Way.prototype = Object.create(this.MapElement.prototype);    
    this.Way.prototype.fillFromObj = function (obj, nodesArray)
    {
        let way = this;
        this.fillBaseFromObj(obj);/* this.nds = obj.nds; */        
        obj.nds.forEach(nd => { way.addNode(nodesArray.where((item) => { return (item.id === nd); }).first()); });
    }

    this.Relation = function (id = 0)
    {
        osm.MapElement.call(this, id, osm.Type_Relation);
        this.members = [];

        this.toString = () => { return `relation${this.id}`; }        
    }
    this.Relation.prototype = Object.create(this.MapElement.prototype);    
    this.Relation.prototype.fillFromObj = function (obj) { this.fillBaseFromObj(obj); this.members = obj.members; }

    this.Icon = function (id = 0)
    {
        osm.MapElement.call(this, id, osm.Type_Relation);
        this.members = [];

        this.toString = () => { return `relation${this.id}`; }
    }
    this.Relation.prototype = Object.create(this.MapElement.prototype);
    this.Relation.prototype.fillFromObj = function (obj) { this.fillBaseFromObj(obj); this.members = obj.members; }

    let _gsStyles = {};
    _gsStyles.node = new ol.style.Style({ image: new ol.style.Circle({ radius: 5, fill: new ol.style.Fill({ color: '#00af00' }) }), zIndex: 30 });
    _gsStyles.nodeNew = new ol.style.Style({ image: new ol.style.Circle({ radius: 5, fill: new ol.style.Fill({ color: '#0000ff' }) }), zIndex: 30 });    
    _gsStyles.nodeChanged = new ol.style.Style({ image: new ol.style.Circle({ radius: 5, fill: new ol.style.Fill({ color: '#ff00ff' }) }), zIndex: 30 });
    _gsStyles.nodeSelected = new ol.style.Style({ image: new ol.style.Circle({ radius: 7, fill: new ol.style.Fill({ color: '#ff0000' }) }), zIndex: 30 });

    _gsStyles.way = new ol.style.Style({ stroke: new ol.style.Stroke({ color: 'green', width: 4 }), fill: new ol.style.Fill({ color: 'rgba(0, 0, 255, 0.1)' }) });    
    _gsStyles.waySelected = new ol.style.Style({ stroke: new ol.style.Stroke({ color: 'red', width: 5 }), fill: new ol.style.Fill({ color: 'rgba(255, 0, 0, 0.2)' }) }); 
    _gsStyles.wayNewSelected = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#ff00ff', width: 4 }), fill: new ol.style.Fill({ color: 'rgba(255, 0, 255, 0.1)' }) });    

    _gsStyles.gs_build = {};
    _gsStyles.gs_build.main = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#6d6d6e', width: 1 }), fill: new ol.style.Fill({ color: '#a5a5a6' }), zIndex: 1 });
    _gsStyles.gs_build.zone0_5 = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#6d6d6e', width: 1 }), fill: new ol.style.Fill({ color: 'rgba(0, 255, 0, 1)' }), zIndex: 20 });
    _gsStyles.gs_build.zone1 = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#6d6d6e', width: 1 }), fill: new ol.style.Fill({ color: 'rgba(255, 255, 0, 1)' }), zIndex: 20 });
    _gsStyles.gs_build.zone1_5 = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#6d6d6e', width: 1 }), fill: new ol.style.Fill({ color: 'rgba(255, 0, 0, 1)' }), zIndex: 20 });
    _gsStyles.gs_build.mainSelected = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#c43a35', width: 1 }), fill: new ol.style.Fill({ color: '#d64c4c' }), zIndex: 1 });    
    
    _gsStyles.gs_way = {};    
    _gsStyles.gs_way.stepway = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#cecece', width: 1 }), fill: new ol.style.Fill({ color: '#cecece' }), zIndex : 2 });
    _gsStyles.gs_way.stepwaySelected = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#e95f5f', width: 1 }), fill: new ol.style.Fill({ color: '#e95f5f' }), zIndex: 2 });

    _gsStyles.gs_path = {};    
    _gsStyles.gs_path.trace = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#000000', width: 4 }), fill: new ol.style.Fill({ color: 'rgba(0, 0, 0, 1)' }), zIndex: 50  });    
    _gsStyles.gs_path.traceSelected = new ol.style.Style({ stroke: new ol.style.Stroke({ color: '#000000', width: 4 }), fill: new ol.style.Fill({ color: 'rgba(0, 0, 0, 1)' }), zIndex: 50  });    

    _gsStyles.gs_icon = {};
    _gsStyles.gs_icon.exit = new ol.style.Style({ image: new ol.style.Icon({ anchor: [8, 8], anchorXUnits: 'pixels', anchorYUnits: 'pixels', opacity: 1, src: '/maps/images/exit.png' }), zIndex: 20 });
    _gsStyles.gs_icon.stairs = new ol.style.Style({ image: new ol.style.Icon({ anchor: [8, 8], anchorXUnits: 'pixels', anchorYUnits: 'pixels', opacity: 1, src: '/maps/images/stairs.png' }), zIndex: 20 });
    
    let _tagsExcept = {};
    _tagsExcept['gs_build'] = ['gs_way'];
    _tagsExcept['gs_way'] = ['gs_build'];
})();

