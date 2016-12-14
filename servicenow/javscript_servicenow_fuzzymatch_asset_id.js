#!/usr/local/bin/js

  'use strict';
  
  var collator;
  try {
    collator = (typeof Intl !== "undefined" && typeof Intl.Collator !== "undefined") ? Intl.Collator("generic", { sensitivity: "base" }) : null;
  } catch (err){
    console.log("Collator could not be initialized and wouldn't be used");
  }
  // arrays to re-use
  var prevRow = [],
    str2Char = [];
  
  /**
   * Based on the algorithm at http://en.wikipedia.org/wiki/Levenshtein_distance.
   */
  var Levenshtein = {
    /**
     * Calculate levenshtein distance of the two strings.
     *
     * @param str1 String the first string.
     * @param str2 String the second string.
     * @param [options] Additional options.
     * @param [options.useCollator] Use `Intl.Collator` for locale-sensitive string comparison.
     * @return Integer the levenshtein distance (0 and above).
     */
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

  // amd
  if (typeof define !== "undefined" && define !== null && define.amd) {
    define(function() {
      return Levenshtein;
    });
  }
  // commonjs
  else if (typeof module !== "undefined" && module !== null && typeof exports !== "undefined" && module.exports === exports) {
    module.exports = Levenshtein;
  }
  // web worker
  else if (typeof self !== "undefined" && typeof self.postMessage === 'function' && typeof self.importScripts === 'function') {
    self.Levenshtein = Levenshtein;
  }
  // browser main thread
  else if (typeof window !== "undefined" && window !== null) {
    window.Levenshtein = Levenshtein;
  }



// examples
// WS-C4506 to -> CAT4506
//
function preparse(s) {
// replace cahracters that we know  are not material
	var t="";
    t=s.replace(/[\[\]]/g, '');
	return t;
}

function store_distances(id, dist)
{

}

function regExpEscape(literal_string) {
    return literal_string.replace(/[-[\]{}()*+!<=:?.\/\\^$|#\s,]/g, '\\$&');
}

// stores the match that is the least distance
// assumes we match the first one if multiple matches
var top_distance=9999;
// for current_asset_id the closest
var closest_match="";

var current_asset_id="";

function iterate_over_detailed_bom(s, id)
{
	var ret_array=[];
	var cleansed_value="";
	var remainingtext="";
	var lines = s.split('\n');
	var matched_line = -1;
	var theline="";
	var reg;
	var splitline;
	var matches;
	var first;
	var arr=[];
	var parsed_id=preparse(id);
	var to_fuzzy_match="";
	var distance_measure;
	debug("ID: " + id.toString());
	current_asset_id=id;
	for(var i = 0;i < lines.length;i++){
		theline=lines[i];
		 reg='(.*)\(' + regExpEscape(id) + '\).*';
		 matches=theline.match(reg);
		 if (matches !== null) { continue; }

         reg='([^\t]+)';
		 splitline=theline.match(reg);
		 if (splitline=== null) { continue; }
		 first=splitline[0];
		 remainingtext=splitline[1];

         // extract () from first
		matches=first.match(/\(([A-Z][A-Za-z0-9\-]*)\)/)
	    if (matches === null) { continue; }
	    to_fuzzy_match=preparse(matches[1]);
	    distance_measure=Levenshtein.get(parsed_id, to_fuzzy_match);

	    if (distance_measure < top_distance)
	    {
	    	top_distance=distance_measure;
	    	closest_match=to_fuzzy_match;
	    }
	    arr.push(to_fuzzy_match, distance_measure);
	    debug("to match with: " + to_fuzzy_match + ", distance: " + distance_measure.toString());
		 // find asset id within
	}

	debug("winner is for: " + id.toString() + " is : " + closest_match + " (" + top_distance.toString() + ")" );
	return arr;
}

function AssertIt(test, v1,v2)
{
  if (v1===v2)
  {
     debug("test: " + test + ": " + true); 	
  }
  else
  {
     debug("test: " + test + ": " + true); 	
  }
}


// MAIN Start
var sow_bill_of_materials;
var detail_bom_info;

// simple example for one asset id
sow_bill_of_materials =  "1.0	WS-4999";
detail_bom_info = "Firewall-Infrastructure-New-Comple (Cat4999)\t2\t$1,837.33\t$   3,674.66\t$  176,383.68\n Firewall-Support line 1\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2\t$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4998-F)\t2\t$     1,837.33\t$   3,674.66\t$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44\t$   648.88\t$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)\t$    470.97\t$   1,883.88\t$  90,426.24\n"
// var detail_bom_info="Firewall-Infrastructure-New-Complex (Cat4999)	2	$1,837.33	$   3,674.66	$  176,383.68\n Firewall-Support line 1	2	$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2	2	$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)	2	$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)	2	$     1,837.33	$   3,674.66	$  176,383.68\n Firewall-Support YYYY	2	$     324.44	$   648.88	$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)	$    470.97	$   1,883.88	$  90,426.24\n"

 var distance = Levenshtein.get('Firewall – Infrastructure – New – Complex ', 'Security – Infrastructure –Firewall – New – Complex');   // 2
 debug("Test1: " + distance);

 iterate_over_detailed_bom(detail_bom_info, "WS-4999");
	
