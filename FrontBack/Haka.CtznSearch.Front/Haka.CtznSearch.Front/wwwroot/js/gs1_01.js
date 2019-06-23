"use strict";
//#region init
Element.prototype.hasClass = function (className) { return this.classList.contains(className); };
Element.prototype.addClass = function (className) { if (!this.hasClass(className)) this.classList.add(className); };
Element.prototype.removeClass = function (className) { if (this.hasClass(className)) this.classList.remove(className); };
Element.prototype.disable = function () { if (!this.hasClass("disabled")) this.addClass("disabled"); };
Element.prototype.enable = function () { if (this.hasClass("disabled")) this.removeClass("disabled"); };
Element.prototype.hide = function () { if (!this.hasClass("hide")) this.addClass("hide"); };
Element.prototype.show = function () { if (this.hasClass("hide")) this.removeClass("hide"); };
Element.prototype.showflip = function () { if (this.hasClass("hide")) this.removeClass("hide"); else this.addClass("hide"); };
Element.prototype.scrollTo = function ()
{
    let el = this; let posX = 0; let posY = 0;
    while (el !== null) { posX += el.offsetLeft; posY += el.offsetTop; el = el.offsetParent; }
    window.scrollTo(posX, posY);
};
Node.prototype.removeFromParent = function () { if (this.parentNode !== null) this.parentNode.removeChild(this); };

Array.prototype.clear = function () { this.splice(0, this.length) };
Array.prototype.remove = function (item) { this.splice(this.indexOf(item), 1); };
Array.prototype.select = function (func) { let result = []; this.forEach(item => { result.push(func(item)); }); return result; };
Array.prototype.where = function (func) { let result = []; this.forEach(item => { if (func(item)) result.push(item); }); return result; };
Array.prototype.first = function () { if (this.length === 0) return undefined; return this[0]; };
Array.prototype.last = function () { if (this.length === 0) return undefined; return this[this.length - 1]; };
Array.prototype.addRange = function (array) { array.forEach(item => { this.push(item); }); };
//#endregion

//#region gs
var gs = new (function (){    
    document.addEventListener("keydown", handlerKeyDownEvent, false);
    function handlerKeyDownEvent(e)
    {
        var keyCode = e.keyCode;
        if ((keyCode === 13)||(keyCode === 27)) {
            if (!gs.getById('mainPopupBlock').hasClass('hide')) gs.hide('mainPopupBlock');
        } 
    }

    var _markErrorTimeouts = {};
    this.markError = function (labelForName)
    {
        if (_markErrorTimeouts[labelForName] !== undefined) return;
        var el = document.querySelector("label[for='" + labelForName + "']");
        if (!el) { gs.Net.bugReport("UnkObjLabelName " + labelForName); return; }
        var was = el.innerHTML;
        el.innerHTML = "<span class='red'>✔ " + was + "</span>";
        _markErrorTimeouts[labelForName] = setTimeout(function () { el.innerHTML = was; _markErrorTimeouts[labelForName] = undefined; }, 700);
    };      
    this.redirect = function (url) { window.location.href = url; };
    this.changeUrl = function (url) { window.history.replaceState({}, document.title, url); };
    this.errorText = function (text = '')
    {
        gs.getById('errorText').innerHTML = text;        
    };
    var popupCallback = undefined;
    this.showMessage = function (text, callback = undefined)
    {
        popupCallback = callback;        
        gs.getById('mainPopupText').innerHTML = text;
        gs.hide('mainPopupInput');
        gs.show('mainPopupBlock');
    };
    this.showInput = function (text, callback)
    {
        popupCallback = callback;
        gs.getById('mainPopupText').innerHTML = text;
        gs.getById('mainPopupInput').value = '';
        gs.show('mainPopupInput');
        gs.show('mainPopupBlock');
    };
    this.closePopup = function ()
    {        
        gs.hide('mainPopupInput');
        gs.hide('mainPopupBlock');
        if (popupCallback) popupCallback(gs.getById('mainPopupInput').value);
    };
    this.setHtml = function (selector, html) { gs.getAll(selector).forEach(function (object) { object.innerHTML = html; }); };
    this.setValue = function (selector, value) { gs.getAll(selector).forEach(function (object) { object.value = value; }); }; 

    this.show = function (id) { this.getById(id).show(); };
    this.hide = function (id) { this.getById(id).hide(); };
    this.showflip = function (id) { this.getById(id).showflip(); };
    this.disable = function (id) { this.getById(id).disable(); };
    this.enable = function (id) { this.getById(id).enable(); };

    this.valueById = function (id) 
    {
        let el = gs.getById(id);
        if (!el) return undefined;
        if (el.value === undefined) { gs.Net.bugReport('ObjNoValId=' + id); return undefined; }
        return el.value;        
    };
    this.htmlById = function (id)
    {
        let el = gs.getById(id);
        if (!el) return undefined;
        if (el.innerHTML === undefined) { gs.Net.bugReport('ObjNoInnerHTMLId=' + id); return undefined; }
        return el.innerHTML;
    };
    this.getById = function (id)
    {
        let el = document.getElementById(id);
        if (!el) { gs.Net.bugReport('UnkObjId=' + id); return undefined; }
        return el;
    };
    this.getAll = function (selector) { return Array.prototype.slice.call(document.querySelectorAll(selector)); };
    this.createUrlArgs = function (data)
    {
        let urlArgs = [];
        for (var key in data) urlArgs.push(`${key}=${encodeURIComponent(data[key])}`);
        return urlArgs.join('&');
    };
    this.getUrlArgs = function () {
        var args = {};
        window.location.search.replace("?", "").split("&").forEach(function (arg)
        {
            let parser = arg.split("=");
            if (parser.length!==2) return;
            args[parser[0]] = decodeURIComponent(parser[1]);
        });
        return args;
    }
    this.addThisHtmlToValue = function (obj)
    {
        let target = gs.getById(obj.parentNode.getAttribute('targetId'));
        let value = target.getAttribute('value');
        target.setAttribute('value', value + obj.innerHTML + '; ');
    };
    this.getInitData = function () { var init = {}; let inputs = gs.getAll('input[type=hidden]').forEach(function (input) { init[input.id] = input.value; }); return init; };
    this.eventPreventDefaults = function (e) { e.preventDefault(); e.stopPropagation(); };
    this.dropZoneInit = function (targetElement, activeHandler, unactiveHandler, dropHandler)
    {
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => { targetElement.addEventListener(eventName, gs.eventPreventDefaults, false); });
        ['dragenter', 'dragover',].forEach(eventName => { targetElement.addEventListener(eventName, activeHandler, false); });
        ['dragleave', 'drop'].forEach(eventName => { targetElement.addEventListener(eventName, unactiveHandler, false); });
        targetElement.addEventListener('drop', dropHandler, false);
    };
    this.getObjectKeys = function (obj) { let key; let result = []; for (key in obj) result.push(key); return result; };
    this.getObjectValues = function (obj) { let key; let result = []; for (key in obj) result.push(obj[key]); return result; };
    this.getShortTime = function (date)
    {
        function addZero(value) { if (value < 10) return "0" + value; return "" + value; }
        return addZero(date.getDate()) + '.' + addZero(date.getMonth()) + '.' + addZero(date.getFullYear()) + ' ' + addZero(date.getHours()) + ':' + addZero(date.getMinutes());        
    }
    this.getData = function (name)
    {
        let el;
        document.querySelectorAll('.dataBlock').forEach(dEl => { if (dEl.getAttribute("data-name" === name)) el = dEl; });
        if (!el) return undefined;
        let data = el.querySelector(".data");
        let defText = data.getAttribute("defText");
        if (!defText) defText = "---";
        if (data.innerText === defText) return "";
        return data.innerText;
    }
    this.getDataFromEl = function (dataEl)
    {
        let defText = dataEl.getAttribute("defText");
        if (!defText) defText = "---";
        if (dataEl.innerText === defText) return "";
        return dataEl.innerText.trim();
    }
    this.getDataObj = function ()
    {
        let result = {};
        document.querySelectorAll('.dataBlock').forEach(dEl =>
        {
            let dName = dEl.getAttribute("data-name"); if (!dName) return;
            let dataEl = dEl.querySelector(".data"); if (!dataEl) return;
            result[dName] = gs.getDataFromEl(dataEl);
        })
        return result;
    }
    this.getDataEl = function (name)
    {
        let el;
        document.querySelectorAll('.dataBlock').forEach(dEl =>
        {
            if (dEl.getAttribute("data-name") === name) el = dEl;
        });
        return el;
    }
    this.clearData = function (name)
    {
        let el = gs.getDataEl(name);
        if (!el) return;
        let defText = el.querySelector(".data").getAttribute("defText");
        if (!defText) defText = "---";
        el.querySelector(".data").innerHTML = defText;
    }


    document.addEventListener("blur", handlerBlurEvent, true);
    function handlerBlurEvent(e)
    {
        let el = e.target;
        if (!el.hasClass) return;
        if (!el.hasClass('data')) return;
        if (!el.parentNode.hasClass('dataBlock')) return;

        let data = el.innerText.replace(/\s+/g, '');
        if (!data) {
            let defText = el.getAttribute("defText");
            if (!defText) defText = "---";
            el.innerHTML = defText;
        }
    }
    document.addEventListener("focus", handlerFocusEvent, true);
    function handlerFocusEvent(e)
    {
        let el = e.target;
        if (!el.hasClass) return;
        if (!el.hasClass('data')) return;
        if (!el.parentNode.hasClass('dataBlock')) return;
        let defText = el.getAttribute("defText");
        if (!defText) defText = "---";
        if (el.innerText === defText) el.innerHTML = '<br/>';
    }


})();
//#endregion

//#region gs.Net
gs.Net = new (function ()
{
    this.post = function (url, data, handler = null)
    {
        var xhttp = new XMLHttpRequest();
        if (handler !== null)
            xhttp.onreadystatechange = function () { if (xhttp.readyState === 4) { if (xhttp.status === 200) handler(JSON.parse(xhttp.responseText)); else handler({ error: 255 }) } };
        let sendData = [];
        if (data !== null)
            for (var key in data) sendData.push(`${key}=${encodeURIComponent(data[key])}`)
        xhttp.open('POST', url, true);
        xhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
        xhttp.send(sendData.join('&'));
    };
    this.get = function (url, handler = null)
    {
        var xhttp = new XMLHttpRequest();
        if (handler !== null)
            xhttp.onreadystatechange = function () { if (xhttp.readyState === 4) { if (xhttp.status === 200) handler(JSON.parse(xhttp.responseText)); else handler({ error: 255 }) } };
        xhttp.open('GET', url, true);
        xhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
        xhttp.send();
    };
    this.postFiles = function (url, data, handler = null)
    {
        var xhttp = new XMLHttpRequest();
        if (handler !== null)
            xhttp.onreadystatechange = function () { if (xhttp.readyState === 4) { if (xhttp.status === 200) handler(JSON.parse(xhttp.responseText)); else handler({ error: 255 }) } };
        xhttp.open('POST', url, true);
        xhttp.send(data);
    };
    this.bugReport = function (message)
    {
        var err = new Error();
        gs.Net.post('/Report/Bug', { 'message': message, 'stack': err.stack });
    };
    this.getErrorByCode = function (resultObj)
    {
        if (resultObj.object != null)
            return getCode(parseInt(resultObj.error)) + " (" + resultObj.object + ")"; else
            return getCode(parseInt(resultObj.error));
        function getCode(errorCode)
        {
            const wrong = "Неправильный";
            switch (errorCode) {
                case 2: return "Уже существует!";
                case 3: return "Объект не найден!";
                case 4: return "Уже сделано!";
                case 5: return "Не подтверждено!";
                case 6: return "Сохранено!";

                case 40: return "Геокод не найден!";
                case 41: return "Категория не найдена!";
                case 42: return "Ключевая фраза не найдена!";
                case 43: return "Точка не найдена!";
                case 44: return "Код не найден!";
                case 46: return "Пользователь не найден!";

                case 62: return wrong + " аргумент!";
                case 63: return wrong + " пароль!";
                case 64: return wrong + " код с картинки!";
                case 65: return wrong + " телефон!";
                case 66: return wrong + " код!";
                case 67: return wrong + " список ключей!";
                case 68: return wrong + " символ этого языка!";

                case 120: return "Слишком много попыток!";
                case 121: return "Вы не можете сделать это.";
                case 122: return "Заблокировано администрацией!";
                case 123: return "Слишком часто. Попробуйте чуть позже.";
                case 124: return "Запрос уже в работе!";
                case 125: return "Превышен лимит!";
                case 126: return "Вам необходимо авторизоваться или зарегистрироваться!";

                case 150: return "Контрольный список содержит неизвестное слово!"

                case 170: return "Недостаточно средств!"
                case 171: return "Неизвестный запрос на перевод!"

                case 200: return "Ошибка формата данных!";
                case 201: return "Ошибка подписи!";
                case 202: return "Ошибка безопасности!";
                case 203: return "Ошибка сессии!";
                case 248: return "Ошибка ответа кросс-сервера!";
                case 249: return "Один из сервисов временно недоступен!";
                case 250: return "Неизвестная ошибка!";
                case 254: return "Ошибка формата json!";
                case 255: return "Потеряно соединение. Проверьте свое подключение!";
                default: return 'Ошибка сервера номер ' + errorCode + '. Пожалуйста, сообщите поддержке.';
            }
        }
    }
})();
window.onerror = function (message, url, lineNumber) { gs.Net.bugReport(`winOnError: ${message}, ${url}:${lineNumber}`); };
//#endregion

//#region gs.OLMap
gs.OLMap = new (function () 
{
    this.EventChangedYouAreHere = 'EventChangedYouAreHere';
    this.EventSelectedPoint = 'EventSelectedPoint';
    var _element;
    var _map;

    var _layers = {};
    var _sources = {};
    var _styles = {};
    var _icons = {};

    var _points = [];

    var _youAreHere;
    var _viewOnYou = false;
    var _selectedPoint;

    var _config;


    function handlerStyleIcons(feature) { return _styles[feature.getId()]; }
    function handlerStyleClusters(features)
    {
        var size = features.get('features').length;
        let imageSize = 10 + (size / _points.length) * 10;
        var color = '#3399CC';
        var text = size.toString();
        if (size === 1) {
            let pointId = features.get('features')[0].getId();
            let point = _points[pointId];
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

    this.transformCoordinatesForOL = function (lonlat) { return ol.proj.fromLonLat(lonlat, 'EPSG:3857'); }
    this.transformCoordinatesForGS = function (lonlat) { return ol.proj.toLonLat(lonlat, 'EPSG:3857'); }
    this.addEventListener = function (event, handler) { _element.addEventListener(event, handler); }
    this.removeEventListener = function (event, handler) { _element.removeEventListener(event, handler); }
    this.showIcon = function (name, coordinate)
    {
        if (!_icons[name]) { gs.Net.bugReport("UnkMapIcon:" + name); return; }
        _icons[name].getGeometry().setCoordinates(coordinate);
        let feature = _sources['icons'].getFeatureById(name);
        if (feature !== null) { return; }
        feature = _icons[name];
        _sources['icons'].addFeature(feature);
    };
    this.hideIcon = function (name)
    {
        let feature = _sources['icons'].getFeatureById(name);
        if (feature === null) { return; }
        _sources['icons'].removeFeature(feature);
    };
    this.addPoint = function (point)
    {
        if (_points[point.Id]) this.removePoint(point.Id);
        _points[point.Id] = point;

        let coordinate = this.transformCoordinatesForOL([parseFloat(point.Options.Longitude), parseFloat(point.Options.Latitude)]);
        let feature = new ol.Feature(new ol.geom.Point(coordinate));
        feature.setId(point.Id);
        point.feature = feature;
    };
    this.removePoint = function (pointId)
    {
        this.hide(pointId);
        _points[pointId] = undefined;
    };
    this.showPoint = function (pointId)
    {
        let exist = _sources['points'].getFeatureById(pointId);
        if (exist === null) _sources['points'].addFeature(_points[pointId].feature);
    };
    this.hidePoint = function (pointId)
    {
        let exist = _sources['points'].getFeatureById(pointId);
        if (exist !== null) _sources['points'].removeFeature(exist);
    };
    this.selectPoint = function (pointId)
    {
        let p = _points[pointId];
        this.showIcon('lock', p.feature.getGeometry().getCoordinates());
        _selectedPoint = p;
        _element.dispatchEvent(new CustomEvent(this.EventSelectedPoint, { "detail": _selectedPoint }));
    };
    this.unselectPoint = function ()
    {
        if (!_selectedPoint) return;
        _selectedPoint = undefined;
        this.hideIcon('lock');
    };
    this.viewOnYouAreHere = function () { _viewOnYou = true; let zoom = this.getZoom(); if (zoom < 13) zoom = 13; _map.getView().animate({ center: _youAreHere, zoom: zoom, duration: 500 }); }
    this.viewOnCoordinate = function (coordinate) { _viewOnYou = false; let zoom = this.getZoom(); if (zoom < 13) zoom = 13; _map.getView().animate({ center: coordinate, zoom: zoom, duration: 500 }); }
    this.getPoint = function (pointId) { return _points[pointId]; }
    this.getPoints = function () { return _points.slice(); }
    this.getSelectedPoint = function () { return _selectedPoint; }
    this.getYouAreHere = function () { return _youAreHere; }
    this.setYouAreHere = function (coordinate) { _youAreHere = coordinate.slice(); if (_viewOnYou) this.viewOnYouAreHere(); this.showIcon('youAreHere', _youAreHere); _element.dispatchEvent(new CustomEvent(this.EventChangedYouAreHere, { "detail": coordinate.slice() })); }
    this.getDistance = function (coord1, coord2) { return Math.round(new ol.geom.LineString([coord1, coord2]).getLength()); }
    this.getZoom = function () { return Math.round(_map.getView().getZoom()); }
    this.getUrlArg = function (coordinate, zoom) { return `${coordinate[1].toFixed(6)}_${coordinate[0].toFixed(6)}_${this.getZoom()}`; }
    this.setHeight = function (heigth) { _element.style.height = heigth; _map.updateSize(); }
    

    this.initialization = function (config)//{mapElementId, initZoom, initCoordinate}
    {        
        ol.inherits(gs.OLMap.Actions.Pointer, ol.interaction.Pointer);
        _config = config;
        if (config.mapElementId) _element = gs.getById(config.mapElementId);
        _sources['icons'] = new ol.source.Vector({ features: [] });
        _sources['points'] = new ol.source.Vector({ features: [] });
        _sources['clusters'] = new ol.source.Cluster({ distance: 30, source: _sources['points'] });

        _layers['osm'] = new ol.layer.Tile({ source: new ol.source.OSM() });
        _layers['clusters'] = new ol.layer.Vector({ source: _sources['clusters'], style: handlerStyleClusters });
        _layers['icons'] = new ol.layer.Vector({ source: _sources['icons'], style: handlerStyleIcons });

        _styles['youAreHere'] = new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 46],
                anchorXUnits: 'fraction',
                anchorYUnits: 'pixels',
                opacity: 1,
                src: '/images/youAreHere.png'
            })
        });
        _styles['lock'] = new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 14],
                anchorXUnits: 'fraction',
                anchorYUnits: 'pixels',
                opacity: 1,
                src: '/images/lock.png'
            })
        });

        _icons['youAreHere'] = new ol.Feature(new ol.geom.Point([0, 0]));
        _icons['youAreHere'].setId("youAreHere");
        _icons['lock'] = new ol.Feature(new ol.geom.Point([0, 0]));
        _icons['lock'].setId("lock");

        _youAreHere = [0, 0];
        if (_element) {
            let initZoom = 1; if (config.initZoom) initZoom = config.initZoom;
            let initCoordinate = [0, 0]; if (config.initCoordinate) initCoordinate = gs.OLMap.transformCoordinatesForOL(config.initCoordinate);
            let actions = { doubleClickZoom: false, altShiftDragRotate: false, pinchRotate: false };
            if (config.actionDragMap === false) { actions["dragPan"] = false; actions["dragZoom"] = false; actions["mouseWheelZoom"] = false; }
            _map = new ol.Map({
                interactions: ol.interaction.defaults(actions).extend([new gs.OLMap.Actions.Pointer()]),
                layers: [_layers['osm'], _layers['clusters'], _layers['icons']],
                controls: ol.control.defaults({ attribution: false }).extend([new ol.control.Attribution({ collapsible: false })]),
                target: _element.id,
                view: new ol.View({
                    center: initCoordinate,
                    zoom: initZoom
                })
            });
        }
    }

    this.Actions = {};
    this.Actions.Pointer = function ()
    {
        ol.interaction.Pointer.call(this, {
            handleDownEvent: gs.OLMap.Actions.Pointer.handleDownEvent,
            handleMoveEvent: gs.OLMap.Actions.Pointer.handleMoveEvent,
            handleUpEvent: gs.OLMap.Actions.Pointer.handleUpEvent,
            handleDragEvent: gs.OLMap.Actions.Pointer.handleDragEvent
        });
        this.lastClickPixel = [0, 0];
        this._coordinate = null;
        this._cursor = 'pointer';
        this._feature = null;
        this._previousCursor = undefined;
    };
    this.Actions.Pointer.handleDownEvent = function (evt)
    {
        var mapEvt = evt.map;
        var feature = mapEvt.forEachFeatureAtPixel(evt.pixel, function (feature) { return feature; });
        if (feature) { this._coordinate = evt.coordinate; this._feature = feature; } else {
            if (_config.actionDragYouAreHere !== false)
                if ((Math.abs(evt.pixel[0] - this.lastClickPixel[0]) + Math.abs(evt.pixel[1] - this.lastClickPixel[1])) < 20) {
                    gs.OLMap.setYouAreHere(evt.coordinate);
                    this.lastClickPixel = [0, 0];
                }
            this.lastClickPixel = evt.pixel;
        }
        return !!feature;
    };
    this.Actions.Pointer.handleDragEvent = function (evt)
    {
        this.lastClickPixel = [0, 0];
        if (this._feature.getId() !== 'youAreHere') return;
        if (_config.actionDragYouAreHere === false) return;
        var deltaX = evt.coordinate[0] - this._coordinate[0];
        var deltaY = evt.coordinate[1] - this._coordinate[1];

        var geometry = this._feature.getGeometry();
        geometry.translate(deltaX, deltaY);

        this._coordinate = evt.coordinate.slice();
        _viewOnYou = false;
        gs.OLMap.setYouAreHere(geometry.getCoordinates());        
    };
    this.Actions.Pointer.handleMoveEvent = function (evt)
    {
        if (this._cursor) {
            this.lastClickPixel = [0, 0];
            var map = evt.map;
            var feature = map.forEachFeatureAtPixel(evt.pixel,
                function (feature)
                {
                    return feature;
                });
            var element = evt.map.getTargetElement();
            if (feature) {
                if (element.style.cursor !== this._cursor) {
                    this._previousCursor = element.style.cursor;
                    element.style.cursor = this._cursor;
                }
            } else if (this._previousCursor !== undefined) {
                element.style.cursor = this._previousCursor;
                this._previousCursor = undefined;
            }
        }
    };
    this.Actions.Pointer.handleUpEvent = function ()
    {
        this._coordinate = null;
        let fs = this._feature.get("features");
        if (fs)
            if (fs.length === 1) { gs.OLMap.selectPoint(fs[0].getId()); }
        this._feature = null;
        return false;
    };  
})();
//#endregion

//#region gs.CheckBox

document.addEventListener('DOMContentLoaded', function ()
{
    document.querySelectorAll(".checkbox").forEach(function (obj)
    {
        let t = new gs.CheckBox.create(obj);
        t = null;
    });
});

gs.CheckBox = new (function ()
{    
    this.EventSelectedItem = 'EventCheckBoxSelectedItem';
    this.EventClickOnItem = 'EventClickOnItem';

    function itemSelect() { if (!this.classList.contains("selected")) this.classList.add("selected"); this.parentNode.dispatchEvent(new CustomEvent(gs.CheckBox.EventSelectedItem, { "detail": this })); };
    function itemUnselect () { if (this.classList.contains("selected")) this.classList.remove("selected"); };
    function itemSelectFlip () { if (this.classList.contains("selected")) this.classList.remove("selected"); else this.classList.add("selected"); };
    function itemGetValueId ()
    {
        let vId = this.getAttribute('valueId');
        if (vId === undefined) {
            let info = ""; if (this.parentNode) info = info + " pId=" + this.parentNode.id; info = info + " html=" + this.innerHTML;
            gs.Net.bugReport('CbANoValueId=' + info); return undefined;
        }
        return parseInt(vId);
    }

    var checkboxExtend = function (node) 
    {
        var _node = node;

        function validItem(item) {
            if (!item.classList.contains("cbItem")) return false;
            if (item.parentNode !== _node) return false;
            item.select = itemSelect;
            item.unselect = itemUnselect;
            item.selectFlip = itemSelectFlip;
            item.getValueId = itemGetValueId;
            return true;
        }
        function handlerCheckboxClick (e) {
            let item = e.target;
            if (!validItem(item)) return;
            if (_node.classList.contains('valueToTarget')) gs.addThisHtmlToValue(item);
            if (_node.classList.contains('unselectable'))
            {
                _node.dispatchEvent(new CustomEvent(gs.CheckBox.EventClickOnItem, { "detail": item }));
                return;
            }
            if (!_node.classList.contains("multiseletion")) { _node.checkbox.unselectAll(); }
            item.selectFlip();
            _node.dispatchEvent(new CustomEvent(gs.CheckBox.EventSelectedItem, { "detail": item }));
            _node.dispatchEvent(new CustomEvent(gs.CheckBox.EventClickOnItem, { "detail": item }));
        };

        _node.addEventListener('click', handlerCheckboxClick);        

        this.unselectAll = function () { this.getItems().forEach(function (item) { item.unselect(); }); };        
        this.getValueId = function ()
        {            
            let selected = this.getSelected();
            if (selected) return selected.getValueId(); else return undefined;
        }        
        this.getValuesId = function ()
        {
            let items = this.getSelectedAll();
            let result = [];
            items.forEach(item => { result.push(item.getValueId()); })
            return result;
        }     
        this.getItemsIds = function ()
        {
            let items = this.getItems();
            let result = [];
            items.forEach(item => { let val = item.getValueId(); if (val) result.push(item.getValueId()); })
            return result;
        }
        this.getSelected = function ()
        {
            let selected = this.getSelectedAll();
            if (selected.length === 0) return undefined;
            return selected[0];
        }        
        this.getSelectedAll = function () { return this.getItems().filter(function (item) { return item.classList.contains("selected"); }); }        
        this.getItems = function () {
            var items = [];
            _node.querySelectorAll('.cbItem').forEach(function (item)
            {
                if (!validItem(item)) return;
                let vId = item.getValueId(); if (vId === undefined) return;
                items.push(item);
            });
            return items;
        }
       
        this.getItemByValueId = function(valueId)
        {
            let items = this.getItems().filter(function (item) { return item.getValueId() === valueId; });
            if (items.length === 0) return undefined;            
            return items[0];
        }
    };
    this.create = function (element)
    {
        element.checkbox = new checkboxExtend(element);
        return element;
    }
})();
//#endregion

//#region gs.FocusVector
gs.FocusVector = new (function ()
{
    var _focusVector = {};    
    function _nextFocusHandler(event)
    {
        if (event.keyCode !== 13) return;
        let vgName = event.target.getAttribute("focusVectorGroup");
        if (!vgName) return;
        if (!_focusVector[vgName]) return;
        let list = _focusVector[vgName];
        for (var i = 0; i < list.length; i++)
            if (list[i] === event.target) {
                let nextId = i + 1;
                if (nextId === list.length) { event.target.blur(); return; } else {
                    let el = list[nextId];
                    if (el.getAttribute('type') === 'submit') { el.click(); return }
                    if (el.classList.contains('btn')) { el.click(); return; }
                    el.focus();
                    return;
                }
            }
    }
    function _clearVector(groupName)
    {
        let list = _focusVector[groupName];
        if (!list) return;        
        list.forEach(function (el)
        {
            el.removeAttribute("focusVectorGroup");
            el.removeEventListener('keyup', _nextFocusHandler);            
        })
        _focusVector[groupName] = undefined;
    }
    this.setVector = function (list, groupName = "noname")
    {
        if (_focusVector[groupName]) _clearVector(groupName);
        _focusVector[groupName] = list;
        list.forEach(function (el)
        {            
            el.setAttribute("focusVectorGroup", groupName);
            el.addEventListener('keyup', _nextFocusHandler);
        })
    }
    this.setVectorByIds = function (list, groupName = "noname")
    {
        if (_focusVector[groupName]) _clearVector(groupName);
        var els = [];
        list.forEach(function (id) { let el = gs.getById(id); if (el) els.push(el); })
        _focusVector[groupName] = els;
        els.forEach(function (el)
        {
            el.setAttribute("focusVectorGroup", groupName);
            el.addEventListener('keyup', _nextFocusHandler);
        })
    }    
})();
//#endregion

//#region gs.PhoneMask
document.addEventListener('DOMContentLoaded', function () { gs.getAll(".phoneMask").forEach(function (el) { gs.PhoneMask.addInputElement(el); }); });

gs.PhoneMask = new (function ()
{
    function setCursorPosition(input) { var pos = 20; input.focus(); if (input.setSelectionRange) input.setSelectionRange(pos, pos); else if (input.createTextRange) { var range = input.createTextRange(); range.collapse(true); range.moveEnd("character", pos); range.moveStart("character", pos); range.select() } }
    function maskPhone(event)
    {
        var input = event.target;
        var matrix = "+7(___)___ ____", i = 0, def = matrix.replace(/\D/g, ""), val = this.value.replace(/\D/g, ""); if (def.length >= val.length) val = def; this.value = matrix.replace(/./g, function (a) { return /[_\d]/.test(a) && i < val.length ? val.charAt(i++) : i >= val.length ? "" : a }); if (event.type === "blur") { if (this.value.length === 2) this.value = "" } else {
            setCursorPosition(input); setTimeout(function () { setCursorPosition(input); }, 0);
        }
    };

    this.addInputElement = function (input)
    {
        input.addEventListener("input", maskPhone, false);
        input.addEventListener("focus", maskPhone, false);
        input.addEventListener("blur", maskPhone, false);
    }


})();
//#endregion

//#region gs.InputFilters
document.addEventListener('DOMContentLoaded', function ()
{
    gs.getAll(".filterInt").forEach(function (el) { gs.InputFilters.setFilter(el, gs.InputFilters.filterInt); });
    gs.getAll(".filterUint").forEach(function (el) { gs.InputFilters.setFilter(el, gs.InputFilters.filterUInt); });
    gs.getAll(".filterHex").forEach(function (el) { gs.InputFilters.setFilter(el, gs.InputFilters.filterHex); });
    gs.getAll(".filterFloat").forEach(function (el) { gs.InputFilters.setFilter(el, gs.InputFilters.filterFloat); });
    gs.getAll(".filterCurrency").forEach(function (el) { gs.InputFilters.setFilter(el, gs.InputFilters.filterCurrency); });
    gs.getAll(".filterUCurrency").forEach(function (el) { gs.InputFilters.setFilter(el, gs.InputFilters.filterUCurrency); });
});
gs.InputFilters = new (function ()
{ 
    this.setFilter = function(textbox, inputFilter)
    {
        ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach(function (event)
        {
            textbox.addEventListener(event, function ()
            {
                if (inputFilter(this.value)) {
                    this.oldValue = this.value;
                    this.oldSelectionStart = this.selectionStart;
                    this.oldSelectionEnd = this.selectionEnd;
                } else if (this.hasOwnProperty("oldValue")) {
                    this.value = this.oldValue;
                    this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
                }
            });
        });
    }
    this.filterInt = function (value) { return /^-?\d*$/.test(value); }
    this.filterUInt = function (value) { return /^\d*$/.test(value); }
    this.filterHex = function (value) { return /^[0-9a-f]*$/i.test(value); }
    this.filterFloat = function (value) { return /^-?\d*[,]?\d*$/.test(value); }
    this.filterCurrency = function (value) { return /^-?\d*[,]?\d{0,2}$/.test(value); }
    this.filterUCurrency = function (value) { return /^\d*[,]?\d{0,2}$/.test(value); }
})();
//#endregion

//#region gs.Validator
gs.Validator = new (function ()
{
    const _sysError = "Системная ошибка. Отчет отправлен тех.поддержке."; 
    function _addError(id, text)
    {
        _valid = false;
        _errorText = _errorText + text + '<br/>';
        gs.markError(id);
    }

    var _valid = true;
    var _errorText = "";
    this.clearValidSession = function () { _valid = true; _errorText = ""; }
    this.validValue = function (id)
    {
        let val = gs.valueById(id);
        if (val === undefined) { _addError(id, _sysError); return undefined; }
        if (!val) { _addError(id, "Введите значение!"); return val; }
        return val;
    }
    this.validCheckBoxValue = function (id)
    {
        let el = gs.getById(id);
        if (!el) { _addError(id, _sysError); return undefined; }
        if (!el.checkbox) { gs.Net.bugReport('UnkCBId=' + id); _addError(id, _sysError); }
        let val = el.checkbox.getValueId();
        if ((val === null) || (val === undefined)) { _addError(id, "Введите значение!"); return val; }
        return val;
    }
    this.validPhone = function (id)
    {        
        let val = this.validValue(id);
        if (!val) { return val; }        
        let phoneReg = /^(8|\+7)(\(\d{3}\))[\d- ]{7,10}$/;
        if (phoneReg.test(val) === false) _addError(id, 'Неверный формат телефона!');
        return val;
    }
    this.validLength = function (id, size)
    {
        let val = this.validValue(id);
        if (!val) { return val; }
        if (val<size) _addError(id, `Не менее ${size} символов!`);                    
        return val;
    }
    this.validSameValue = function (id, sameValue)
    {
        let val = this.validValue(id);
        if (!val) { return val; }
        if (val !== sameValue) _addError(id, `Неодинаковое значение!`);
        return val;
    }
    this.validUint = function (id)
    {
        let val = this.validValue(id);
        if (!val) { return val; }
        let intval = parseInt(val);
        if (intval < 0) _addError(id, `Это не целое положительное число!`);
        if (isNaN(intval)) _addError(id, `Это не целое положительное число!`);
        return intval;
    }
    this.addError = function (id, text) { _addError(id, text); }
    this.getValid = function () { return _valid; }
    this.getErrorText = function () { return _errorText; }
})();
//#endregion

//#region gs.img
gs.img = new (function ()
{
    this.Size62 = '62';
    this.Size124 = '124';
    this.Size310 = '310';
    this.Size620 = '620';    
    this.objUrl = function (obj, objId, imgId, size)
    {
        return this.getServerUrl() + `/images/${obj}/${objId}/${objId}_${imgId}_${size}x${size}.jpg`;
    }
    this.pointUrl = function (pointId, imgId, size)
    {
        return this.getServerUrl() + `/images/point/${pointId}/${pointId}_${imgId}_${size}x${size}.jpg`;
    }
    this.getServerUrl = function () { return `https://${window.location.host}/files`; }

})();
//#nedregion

//#region gs.html
gs.CodeReader = new (function ()
{
    var _callback;
    var _canvasEl;
    var _canvas;
    var _video;   
    var _statusEl;    
    /*
    
    var outputContainer = document.getElementById("output");
    var outputMessage = document.getElementById("outputMessage");
    var outputData = document.getElementById("outputData");
    */
    function drawLine(begin, end, color)
    {
        _canvas.beginPath();
        _canvas.moveTo(begin.x, begin.y);
        _canvas.lineTo(end.x, end.y);
        _canvas.lineWidth = 4;
        _canvas.strokeStyle = color;
        _canvas.stroke();
    }

    

    function tick()
    {
        _statusEl.innerText = "Загружается..."
        if (_video.readyState === _video.HAVE_ENOUGH_DATA) {
            _statusEl.hidden = true;
            _canvasEl.hidden = false;
            //outputContainer.hidden = false;

            _canvasEl.height = _video.videoHeight;
            _canvasEl.width = _video.videoWidth;
            _canvas.drawImage(_video, 0, 0, _canvasEl.width, _canvasEl.height);
            var imageData = _canvas.getImageData(0, 0, _canvasEl.width, _canvasEl.height);
            var code = jsQR(imageData.data, imageData.width, imageData.height, {
                inversionAttempts: "dontInvert",
            });
            if (code) {
                drawLine(code.location.topLeftCorner, code.location.topRightCorner, "#FF3B58");
                drawLine(code.location.topRightCorner, code.location.bottomRightCorner, "#FF3B58");
                drawLine(code.location.bottomRightCorner, code.location.bottomLeftCorner, "#FF3B58");
                drawLine(code.location.bottomLeftCorner, code.location.topLeftCorner, "#FF3B58");
                if (code.data != '')
                {
                    _callback(code.data);
                    gs.CodeReader.hide();
                    return;
                }
                /*outputMessage.hidden = true;
                outputData.parentElement.hidden = false;                
                outputData.innerText = code.data;*/
            } else {
                /*outputMessage.hidden = false;
                outputData.parentElement.hidden = true;*/
            }
        }
        requestAnimationFrame(tick);
    }
    this.getGeoCode = function (callback)
    {
        this.getQR(function (code)
        {
            if (!code) { callback(undefined); return; }
            let parser = code.split('/');
            callback(parser[parser.length - 1]);
        });
    }
    this.getQR = function (callback)
    {
        gs.show('codeReader');     
        _callback = callback;  
        // Use facingMode: environment to attemt to get the front camera on phones


        function handlerOk(stream)
        {
            _canvasEl = gs.getById('codeReaderCanvas');
            _canvas = _canvasEl.getContext("2d");
            _video = document.createElement("video");
            _statusEl = gs.getById('codeReaderStatusMessage');

            _video.srcObject = stream;
            _video.setAttribute("playsinline", true); // required to tell iOS safari we don't want fullscreen
            _video.play();
            requestAnimationFrame(tick);
        }
        function handlerFail()
        {
            gs.CodeReader.hide();
            gs.showMessage('Нет доступной камеры или заблокирован доступ (обновите страницу и разрешите ее использование).');
        }

        navigator.getUserMedia = navigator.getUserMedia ||
            navigator.webkitGetUserMedia ||
            navigator.mozGetUserMedia;

        if (navigator.getUserMedia) {
            navigator.getUserMedia({ video: { facingMode: "environment" }}, handlerOk, handlerFail);                
        } else {
            navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } }).then(handlerOk, handlerFail);
        }
    };
    this.hide = function ()
    {
        if (_video) _video.srcObject.getTracks().forEach(track => track.stop());
        _callback(undefined);
        _callback = undefined;
        gs.hide('codeReader');        
    };  

})();
//#nedregion

//#region gs.html
gs.html = new (function ()
{
    function addAttrs(el, attrs) { if (!attrs) return; for (var attr in attrs) { el.setAttribute(attr, attrs[attr]); } }
    function textContainer(tag, inner, attrs)
    {
        var el = document.createElement(tag);
        el.innerHTML = inner;
        addAttrs(el, attrs);
        return el;
    }
    this.span = function (inner, attrs) { return textContainer('span', inner, attrs); };
    this.div = function (inner, attrs) { return textContainer('div', inner, attrs); };
    this.p = function (inner, attrs) { return textContainer('p', inner, attrs); };
    this.a = function (inner, attrs) { return textContainer('a', inner, attrs); };
    this.div = function (inner, attrs) { return textContainer('div', inner, attrs); };
    this.br = function (attrs) { let el = document.createElement('br'); addAttrs(el, attrs); return el; };
    this.hr = function (attrs) { let el = document.createElement('hr'); addAttrs(el, attrs); return el; };
    this.img = function (attrs) { let el = document.createElement('img'); addAttrs(el, attrs); return el; };

    this.addText = function (el, text) { el.appendChild(document.createTextNode(text)); };
    this.addSpace = function (el) { el.appendChild(document.createTextNode(' ')); }

    this.htmlDecode = function (text) { return this.p(text).textContent; };
    
})();
//#nedregion
