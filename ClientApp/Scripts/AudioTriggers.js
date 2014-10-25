xmlhttp = new XMLHttpRequest();
xmlhttp.open("GET", "/Audio/AudioTriggers.xml", false);
xmlhttp.send();
triggersXml = xmlhttp.responseXML;

var xmlLocations = triggersXml.getElementsByTagName("locations")[0];

var getAudioForLocation = function(locationId) {
    if (xmlLocations.getElementsByTagName(locationId).length != 0) {
        return xmlLocations.getElementsByTagName(locationId)[0].childNodes[0].nodeValue;
    }
    
    return null;
}