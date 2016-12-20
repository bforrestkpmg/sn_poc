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
    var lines = inputstrings.split('\n');
    var res_array=[];
    // we build up based on regex matches 
    var respdata_array=[];

    for(var i = 0;i < lines.length;i++){
          theline=lines[i];
      respdata_array=[];
      reg='([^]*)(' + regex + ')([^]*)';
      //reg=regex;
      matches=theline.match(reg);
     if (matches !== null) {
      respdata_array[0] = matches[2];
      respdata_array[1] = matches[1];
      respdata_array[2] = matches[3];
     }
      else
      {
      respdata_array[0] = "";
      respdata_array[1] = theline;
      respdata_array[2] = "";
    }
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
    if(typeof(skip_fuzzy)==='undefined') tune_distance = 100;
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

// uses preparsed array
// goes to index foudn by same or closest match then reads everything to the the 
// next regex match
// input is the 
// [[ "WX-C9999-E", "before1 "," hello there how"], ["", "2are you", ""], ["WX-C7777-E", "before3 ", "i am fine"]];
// e.g. [[x,y,z],[x,y,z]]
// where x = matched asset id, y = contenet before, z = content after
find_item_details_for_sow_id: function(idx, regex_preparsed_array) {
  var component_info = "";
  var parsed_id;
  var in_block=true;
  var all_content_for_asset_id=[];
  var theline_arr=[];
  component_info="";

  all_content_for_asset_id[0]=regex_preparsed_array[idx][0];
  component_info=regex_preparsed_array[idx][1] + " " + regex_preparsed_array[idx][2]  + ", ";
  for(var i = idx+1;i < regex_preparsed_array.length;i++){
    theline_arr=regex_preparsed_array[i];
    // console.log(theline_arr);
    if (theline_arr[0] === "")
    {
      // component if is before & after content
       component_info += theline_arr[1] + " " + theline_arr[2] + ", ";
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
  all_content_for_asset_id[1]=component_info;
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
  for (i = 0; i < list_of_sows.length; i++) { 
     si=list_of_sows[i];
     id_description_pair=this.wrapper_find_item_details_for_sow_id(si, sow_quote_costs);
     // console.log(id_description_pair);
     if (id_description_pair=== null) continue;
     // add recorded entry to our list
     ret_array.push(id_description_pair);
  }
  return ret_array;
} //get_sow_asset_ids_description_from_bom

}; //OrchParser Class
