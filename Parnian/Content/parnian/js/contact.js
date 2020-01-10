$(document).ready(function () {
    // Google map
    //---------------------------------------------------------------------------
    google.maps.event.addDomListener(window, 'load', function () {
        var lat = 35.6991237,
            lng = 51.3994153;

        //var lat = 35.71789270272572,
        //    lng = 51.41029521981352;

        var myOptions = {
            zoom: 16,
            center: new google.maps.LatLng(lat, lng),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("gmap_canvas"), myOptions);
        marker = new google.maps.Marker({
            map: map,
            position: new google.maps.LatLng(lat, lng)
        });
        infowindow = new google.maps.InfoWindow({ content: "<b> &#1662;&#1585;&#1606;&#1740;&#1575;&#1606; &#1575;&#1606;&#1583;&#1740;&#1588;&#1607; &#1607;&#1606;&#1585;</b><br/>&#1578;&#1607;&#1585;&#1575;&#1606;&#1548; &#1576;&#1575;&#1604;&#1575;&#1578;&#1585; &#1575;&#1586; &#1605;&#1740;&#1583;&#1575;&#1606; &#1608;&#1604;&#1740;&zwnj;&#1593;&#1589;&#1585;(&#1593;&#1580;)&#1548; &#1576;&#1593;&#1583; &#1575;&#1586; &#1586;&#1585;&#1578;&#1588;&#1578; &#1588;&#1585;&#1602;&#1740;&#1548; &#1705;&#1608;&#1670;&#1607;&zwnj;&#1740; &#1662;&#1586;&#1588;&#1705;&#1662;&#1608;&#1585;&#1548; &#1662;&#1604;&#1575;&#1705;&#1778;&#1781;&#1548; &#1591;&#1576;&#1602;&#1607;&zwnj;&#1740; &#1779;&#1548; &#1608;&#1575;&#1581;&#1583; &#1783;<br/> " });
        google.maps.event.addListener(marker, "click", function () {
            infowindow.open(map, marker);
        });
    });

    // Email
    //---------------------------------------------------------------------------
    $("#emailForm").submit(function (event) {
        event.preventDefault();
        var Form = $(this);
        if (Form.valid()) {
            $.post(Form.attr("action"), Form.serialize(), function (response) {
                bootbox.dialog({
                    title: response.title,
                    message: response.text
                });
            });
            return false;
        }
    });
});