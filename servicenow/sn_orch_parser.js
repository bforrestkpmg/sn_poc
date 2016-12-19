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
      reg='(' + regex + ')(.*)';
      //reg=regex;
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
},

find_item_details_for_sow_id: function(id, description_text) {
  var matches;
  var desc_minus_numbers;
  var regExp_headers;
   var matches_of_description;
   // array [0] = asset id [1] = is all coponents that are part of that asset in single line comma separated
   var all_content_for_asset_id = [];
   var component_info = "";
   var in_block;

   var lines = description_text.split('\n');
   in_block=false;
  for(var i = 0;i < lines.length;i++){
    theline=lines[i];
    // have we found the top level item e.g.  description blah (asset id) qty price etc....
    reg='(.*)\(' + regExpEscape(id) + '\).*';

      matches=theline.match(reg);

      // yes so record this asset, now mark the fact we keep going
       if (matches !== null) {
        in_block = true;
        all_content_for_asset_id[0] = id;
        continue; }

      // get all stuff before price / qty / etc.
      if (in_block) {
      reg='([ A-Za-z\t0-9\-]+)\t[0-9]+';
      // reg='(.*)\t(.*)';
        matches=theline.match(reg);
        if (matches !== null) {
          component_info = component_info + ", " + matches[1];
          continue;
        }
        // test for the next top line i.e. an asset descripton with (xxxxxx) in the line
         matches=theline.match(/.*\([A-Za-z0-9\-]+\).*/)
         // we have now found the next top line item so finish this search
         if (matches !== null) { 
          in_block=false;
          break;
         }
      }
   } // for
  all_content_for_asset_id[1]=component_info;
 console.log("find item result");
 console.log(all_content_for_asset_id);
  return all_content_for_asset_id
}, // find_item_details_for_sow_id

find_sow_ids_in_quote_costs: function (bom_str) {
       console.log(bom_str);
  var matches;
  var allmatches = [];
  // matches=bom_str.match(/([1-9][0-9]*\.0\t([A-Z][A-Za-z0-9\-]*)\t)+/);
       // console.log(matches);
  var lines = bom_str.split('\n');
    for(var i = 0;i < lines.length;i++){
       // console.log("line counter: " + i.toString());
        matches=lines[i].match(/[1-9][0-9]*\.0\t([A-Z][A-Za-z0-9\-]*)\t(.+)/);
        if (matches === null) { continue; }
     // console.log("find_sow_ids_in_quote_costs matches: " + matches[1].toString());
        //code here using lines[i] which will give you each line
       allmatches.push(matches[1]);
    }
  // debug(allmatches.length.toString());
  // debug("all matches:");
  // debug(allmatches.toString());
  return allmatches;
}


}; //OrchParser