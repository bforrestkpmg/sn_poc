var asset_id = null;
var closest_asset_id = null;
var distance = null;
var pre_parsed_content = null; // 2D array, each elment = [fuzzy matched asset id, "content"]

 function regExpEscape(literal_string) {
    return literal_string.replace(/[-[\]{}()*+!<=:?.\/\\^$|#\s,]/g, '\\$&');
};
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
    var lines = inputstrings.split('\n');
    // 2D array, each elment = [fuzzy matched asset id, "content"
    var res_array=[];
    // we build up based on regex matches 
    var respdata_array=[];

    for(var i = 0;i < lines.length;i++){
          theline=lines[i];
      respdata_array=[];
      reg='(.*' + regExpEscape(regex) + ')(.*)';
      matches=theline.match(reg);
     if (matches !== null) {
      respdata_array[0] = matches[1];
      respdata_array[1] = matches[2];
     }
      else
      {
      respdata_array[0] = "";
      respdata_array[1] = theline;
    }
    res_array.push(respdata_array);
    }
   return res_array;
}

}; //OrchParser