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
    t=s.replace(/\[\-\]/g, '');
    t=t.replace();
	return t;
}


function iterate_over_detailed_bom(s)
{
	var ret_array=[];
	var cleansed_value="";
	var remainingtext="";
	var lines = s.split('\n');
	for(var i = 0;i < lines.length;i++){
	   ret_array.push([cleansed_value, remainingtext]);

	}
	return ret_array;
}



// MAIN Start
var sow_bill_of_materials;
var detail_bom_info;

// simple example for one asset id
sow_bill_of_materials =  "1.0	WS-c4999	Cat4500 ";
detail_bom_info = "Firewall-Infrastructure-New-Complex (WX-C9999-X)	2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support line 1\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2\t$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)\t2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44	$   648.88	$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)	$    470.97 $   1,883.88	$  90,426.24\n"

 var distance = Levenshtein.get('Firewall – Infrastructure – New – Complex ', 'Security – Infrastructure –Firewall – New – Complex');   // 2
 debug("Test1: " + distance);
