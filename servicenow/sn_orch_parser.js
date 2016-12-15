var asset_id = null;
var closest_asset_id = null;
var distance = null;
var pre_parsed_content = null; // 2D array, each elment = [fuzzy matched asset id, "content"]

var OrchParser = {


 setup: function(the_asset_id) {
    asset_id = the_asset_id;
    closest_asset_id = null;
    distance = null;
    pre_parsed_content = null; // 2D array, each elment = [fuzzy matched asset id, "content"]
},

 preparse_asset_id: function(s) {
        var t="";
        t=s.replace(/[\[\]-]/g, '');
        return t;
},
 preparse_array_of_strings: function(regex, inputstrings) {
        return ["x","hello"];
}

}; //OrchParser