function showOnpage(contentDiv, pageUrl, elementName, elementPrettyName, nodeID, pubID, versionID) {
    var url = pageUrl + "VersionID=" + versionID + "&PubID=" + pubID + "&elementName=" + elementName + "&contentDivID=" + contentDiv.id + "&ts=" + Date.parse(new Date());
    var iframe = "<div id=\"DialogEdit\" title='" + elementPrettyName + "'><iframe src='" + url + "' width=100% height=99% marginWidth=0 marginHeight=0 frameBorder=0 scrolling=no></iframe></div>";
    OpenDialog("#DialogEdit", iframe);
}

function showOnpageNew(pageUrl, elementPrettyName, parentID, nodeID, pubID, pageType) {
    var url = pageUrl + "ParentID=" + parentID + "&NodeID=" + nodeID + "&PubID=" + pubID + "&contentDivID=DialogNewPage&mode=new&PageType=" + pageType + "&ts=" + Date.parse(new Date());
    var iframe = "<div id=\"DialogEdit\" title='" + elementPrettyName + "'><iframe src='" + url + "' width=100% height=99% marginWidth=0 marginHeight=0 frameBorder=0 scrolling=no></iframe></div>";
    OpenDialog("#DialogEdit", iframe);
}

function OpenDialog(id, div) {
    $(document).ready(function () {
        $("body").append(div);
        $(id).dialog({
            height: 600,
            width: 870,
            close: closeOnPage
        });
    });
}

function closeOnPage() {
    $("#DialogEdit").dialog('destroy');
    $("#DialogEdit").remove();
    window.location.reload(true);
}

function onPageElementMouseOver(el) {
    $(document).ready(function () {
        var jEl = $(el);
        jEl.css("margin", "-1px");
        jEl.css("border", "1px dashed red");
    });
}

function onPageElementMouseOut(el) {
    var jEl = $(el);
    jEl.css("margin", "0");
    jEl.css("border", "none");
}