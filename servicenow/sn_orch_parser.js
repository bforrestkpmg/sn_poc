// TODO put this into OrchParser
// WARNING these are used within the object but cannot work out syntax to include in OrchParser
  var closest_asset_id = null; // this is the id in () that has been extracted
  var closest_distance = 99999; // levenstein distance for each id we try and match
  var distance_measure;
  var to_fuzzy_match="";


 function regExpEscape(literal_string) {
    return literal_string.replace(/[-[\]{}()*+!<=:?.\/\\^$|#\s,]/g, '\\$&');
};
var OrchParser = {
 setup: function() {
},

get_levi: function(x,y,z)
{

levi=(function() {
  
  var collator;
  try {
    collator = (typeof Intl !== "undefined" && typeof Intl.Collator !== "undefined") ? Intl.Collator("generic", { sensitivity: "base" }) : null;
  } catch (err){
    console.log("Collator could not be initialized and wouldn't be used");
  }
  // arrays to re-use
  var prevRow = [], str2Char = [];
  
  /**
   * Based on the algorithm at http://en.wikipedia.org/wiki/Levenshtein_distance.
   */
  var Levenshtein = {
    get: function(str1, str2, options) {
      var useCollator = (options && collator && options.useCollator);
      
      var str1Len = str1.length,
        str2Len = str2.length;
      
      // base cases
      if (str1Len === 0) return str2Len;
      if (str2Len === 0) return str1Len;

      // two rows
      var curCol, nextCol, i, j, tmp;

      // initialise previous row
      for (i=0; i<str2Len; ++i) {
        prevRow[i] = i;
        str2Char[i] = str2.charCodeAt(i);
      }
      prevRow[str2Len] = str2Len;

      // calculate current row distance from previous row
      for (i=0; i<str1Len; ++i) {
        nextCol = i + 1;
        
        for (j=0; j<str2Len; ++j) {
          curCol = nextCol;

          // substution
          var strCmp = useCollator ? (0 === collator.compare(str1.charAt(i), String.fromCharCode(str2Char[j]))) : str1.charCodeAt(i) === str2Char[j];
            
          nextCol = prevRow[j] + ( strCmp ? 0 : 1 );
          
          // insertion
          tmp = curCol + 1;
          if (nextCol > tmp) {
            nextCol = tmp;
          }
          // deletion
          tmp = prevRow[j + 1] + 1;
          if (nextCol > tmp) {
            nextCol = tmp;
          }

          // copy current col value into previous (in preparation for next iteration)
          prevRow[j] = curCol;
        }

        // copy last col value into previous (in preparation for next iteration)
        prevRow[j] = nextCol;
      }

      return nextCol;
    }

  };
  return Levenshtein;
}());

return levi.get(x,y,z);
},


// generates 
// arr[ [x,y,z], [x,y,z] ]
// where
// x = regex extracted item
// y = levenshein distance to id
// z = extracted information 
// e.g. we are effecgively 'exitending' the prepare array
// returns the integer of the closest match

 preparse_asset_id: function(s) {
        var t="";
        t=s.replace(/[\[\]-]/g, '');
        return t;
},

calc_fuzzy_match_to_regex_list: function(id,preparse_array)
{
    var res_array=[];
    var items_array=[];
    var current_preparse_item=[];
    var distance_levenstein_between_id_and_regex_extracted=null;
    for(var i = 0;i < preparse_array.length;i++){
      items_array=[];
      current_preparse_item=preparse_array[i];
      items_array[0]=current_preparse_item[0];
      items_array[2]=current_preparse_item[1];

      distance_levenstein_between_id_and_regex_extracted=this.get_levi(id, items_array[0]);
      items_array[1]=distance_levenstein_between_id_and_regex_extracted;
     res_array.push(items_array);
    }

    return res_array;
},

get_closest_match_from_fuzzy_match_list: function(fuzzy_list)
{
  var closest=99999;
  var item_idx;
  for(var i = 0;i < fuzzy_list.length;i++){
    if (fuzzy_list[i][1] < closest)
    {
       closest=fuzzy_list[i][1];
       item_idx=i;
    }
  }
  if (closest===99999) { return null };
  return fuzzy_list[item_idx][0];
},


    // 2D array, each elment = [exracted matching regex, "content before match", "content after match"]
    // OR ["" no matching regex, line, ""]
 preparse_array_of_strings: function(regex, inputstrings) {
  // console.log(inputstrings);
    var lines = inputstrings.split('\n');
    var res_array=[];
    // we build up based on regex matches 
    var respdata_array=[];
    var matches;
    var matches_split;

    for(var i = 0;i < lines.length;i++){
          theline=lines[i];
      respdata_array=[];
      reg=regex;
      // reg='([^]*)' + regex + '([^]*)';
      // reg=/\(([A-Za-z0-9\-]*)\)/;
      matches=theline.match(reg);
      matches_split=theline.split(reg);
      // console.log(theline);
      // console.log("matches");
      // console.log(matches);
      // console.log("matches_split");
      // console.log(matches_split);
     if ((matches !== null) && (matches_split !== null)) {
      respdata_array[0] =  matches[0];
      respdata_array[1] = matches_split[0];
      respdata_array[2] = matches_split[1];
     }
      else
      {
      respdata_array[0] = "";
      respdata_array[1] = theline;
      respdata_array[2] = "";
    }
    // console.log(respdata_array);
    res_array.push(respdata_array);
    }
   return res_array;
},

// returns index of match or closes
// iterates through array 
// returns index of array if closest or exact match
// null if nothing found
//
// TODO figure out what hte tune distance is for various type of matches
//
find_closest_or_exact_match: function(id, regex_preparsed_array, tune_distance) {
   if(typeof(tune_distance)==='undefined') tune_distance = 100;
   var closest_distance=tune_distance;
   var distance_measure;
   var to_fuzzy_match;
   var id_to_look_at;
   var match_index=null;
    for(var i = 0;i < regex_preparsed_array.length;i++){
      id_to_look_at=regex_preparsed_array[i][0];

       parsed_id=this.preparse_asset_id(id);
       to_fuzzy_match=this.preparse_asset_id(id_to_look_at);
       distance_measure=this.get_levi(parsed_id, to_fuzzy_match);
    // console.log("distance: " + closest_distance + "(for " + parsed_id + ", to: " + to_fuzzy_match + ")");
        if (distance_measure < closest_distance)
        {
          match_index=i;
          closest_distance=distance_measure;
          closest_asset_id=to_fuzzy_match;
        }
    } // for
    return match_index;
}, // find_closes_or_exact_match


split_components_further: function(str, reg){

// var txtpattern = "(.*)" + reg; + "(.*)";
var txtpattern = reg;
var regex = new RegExp(txtpattern);
var result = str.split(regex);

   // console.log(result);
   if (result=== null) { return [str]; }
   return result;
},
// uses preparsed array
// goes to index foudn by same or closest match then reads everything to the the 
// next regex match
// input is the 
// [[ "WX-C9999-E", "before1 "," hello there how"], ["", "2are you", ""], ["WX-C7777-E", "before3 ", "i am fine"]];
// e.g. [[x,y,z],[x,y,z]]
// where x = matched asset id, y = contenet before, z = content after
//array_of_indexes specifies which part of the match we include in component info
find_item_details_for_sow_id: function(idx, regex_preparsed_array, further_regex_split, array_of_index) {
  // console.log(regex_preparsed_array);
  if(typeof(array_of_index)==='undefined') array_of_index = [1,2];
  if(typeof(further_regex_split)==='undefined') further_regex_split = "\t";
  var component_info = "";
  var incoming_compontent_info = [];
  var parsed_id;
  var in_block=true;
  var all_content_for_asset_id=[];
  var theline_arr=[];
  component_info="";

  all_content_for_asset_id[0]=regex_preparsed_array[idx][0];
  component_info=regex_preparsed_array[idx][1]  + ", ";
  // console.log("before loop");
  // console.log(component_info);
  for(var i = idx+1;i < regex_preparsed_array.length;i++){
    theline_arr=regex_preparsed_array[i];
     // console.log("looking at");
     // console.log(theline_arr[0]);
    if (theline_arr[0] === "")
    {
      // TODO lets allow us to use different part of the matches
      // component if is before & after content
      // for (var j=0; j < array_of_index; j++)
      // {
      //  component_info += theline_arr[array_of_index[j]] + " ";
      // }
      // component_info += ", ";
      // console.log("theline_arr");
      // console.log(theline_arr);
      //component_info += theline_arr[1] + ", ";

      incoming_compontent_info=this.split_components_further(theline_arr[1],  "\t");
      // console.log(incoming_compontent_info);

      // FOR NOW just take the 'after'
      // console.log("Adding:");
      // console.log(incoming_compontent_info[0]);
      component_info = component_info + incoming_compontent_info[0]+ ", ";
      // console.log("component_info: ");
      // console.log(component_info);
      // only add comma if we're not the last item 

       // if (i < regex_preparsed_array.length-1) {
       //  if (regex_preparsed_array[i+1][0] === "") {
       //    component_info += component_info + ", ";
       //  }
       // }
    }
    else {
      break;
    }
   } // for
   // console.log("final:");
   //    console.log(component_info);
  all_content_for_asset_id[1]=component_info;
  // console.log("returning");
  // console.log(all_content_for_asset_id);
  return all_content_for_asset_id
}, // find_item_details_for_sow_id


find_sow_ids_in_quote_costs: function (bom_str) {
  var matches;
  var allmatches = [];
  var lines = bom_str.split('\n');
    for(var i = 0;i < lines.length;i++){
       // console.log("line counter: " + i.toString());
        matches=lines[i].match(/[1-9][0-9]*\.0\t([A-Z][A-Za-z0-9\-]*)\t(.+)/);
        if (matches === null) { continue; }
        //code here using lines[i] which will give you each line
       allmatches.push(matches[1]);
    }
  return allmatches;
},

// for each SOW ID we need to find where it ocurs in the sow_quote_costs
// now match 1.0, 11.0, 2.0 etc. and text after it until tab
// e.g. 1.0 AS-JASDF\tblah blahblah
// assumes
// new line before 1.0 XXXX 
// space between 1.0 AND XXXX
// text up until cell
// TODO what other characters in the regex header
get_sow_asset_ids_description_from_bom: function (list_of_sows, sow_quote_costs) {
  var ret_array=[];
  var id_description_pair;
  var arr_strings = [];
  var fuzzy_match_pre_parse = [];
  var idx_of_closest;
  var content;

  var final_response = [];
  var asset_and_details=[];
  var res;

  var pre_parsed_sow_quote_costs = [];

  for (i = 0; i < list_of_sows.length; i++) { 
     si=list_of_sows[i];
     pre_parsed_sow_quote_costs = this.preparse_array_of_strings(si, sow_quote_costs);
      console.log(pre_parsed_sow_quote_costs);

     fuzzy_match_pre_parse=this.calc_fuzzy_match_to_regex_list(si, pre_parsed_sow_quote_costs);
     // console.log(fuzzy_match_pre_parse);

     idx_of_closest=this.find_closest_or_exact_match(si, fuzzy_match_pre_parse,100);
     // console.log(idx_of_closest);

     asset_and_details=this.find_item_details_for_sow_id(idx_of_closest, pre_parsed_sow_quote_costs);
       // console.log(asset_and_details);
     final_response.push(asset_and_details);
  }
  return final_response;
} //get_sow_asset_ids_description_from_bom

}; //OrchParser Class
