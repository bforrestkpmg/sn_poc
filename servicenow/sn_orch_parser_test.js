var expect = chai.expect;


// starting frmo scratch with https://nicolas.perriault.net/code/2013/testing-frontend-javascript-code-using-mocha-chai-and-sinon/

// describe("Cow", function() {
//   describe("constructor", function() {
//     it("should have a default name", function() {
//       var cow = new Cow();
//       expect(cow.name).to.equal("Anon cow");
//     });

//     it("should set cow's name if provided", function() {
//       var cow = new Cow("Kate");
//       expect(cow.name).to.equal("Kate");
//     });
//   });

//   describe("#greets", function() {
//     it("should throw if no target is passed in", function() {
//       expect(function() {
//         (new Cow()).greets();
//       }).to.throw(Error);
//     });

//     it("should greet passed target", function() {
//       var greetings = (new Cow("Kate")).greets("Baby");
//       expect(greetings).to.equal("Kate greets Baby");
//     });
//   });
// });
// Warn if overriding existing method
function compare_arrays(arr1,array) {
    // if the other array is a falsy value, return
    if (!array)
        return false;

    // compare lengths - can save a lot of time 
    if (arr1.length != array.length)
        return false;

    for (var i = 0, l=arr1.length; i < l; i++) {
        // Check if we have nested arrays
        if (arr1[i] instanceof Array && array[i] instanceof Array) {
            // recurse into the nested arrays
            if (!compare_arrays(arr1[i],array[i])) 
                return false;       
        }           
        else if (arr1[i] != array[i]) { 
            // Warning - two different object instances will never be equal: {x:20} != {x:20}
            return false;   
        }           
    }       
    return true;
}

describe("OrchParser", function() {
  describe("constructor", function() {
    it("defaults and sets asset id", function() {
      OrchParser.setup("assetid");
      expect(asset_id).to.equal("assetid");
      expect(closest_asset_id).to.equal(null);
      expect(distance).to.equal(null);
      expect(pre_parsed_content).to.equal(null);
    });

    it("should preparse asset ids by stripping out characters", function() {
      expect(OrchParser.preparse_asset_id("hellow-][there")).to.equal("hellowthere");
    });
    it("should create array of preparsed content", function() {
      var input_str = "1. hello there how\n2are you\n3.i am fine";
      var expected_arr = [["1."," hello there how"],["","2are you"],["3.","i am fine"]];
      var res=OrchParser.preparse_array_of_strings("[0-9]\\.", input_str);
      var compare_res=compare_arrays(res, expected_arr);
      expect(compare_res).to.equal(true);
    });
    it("should handle empty strings", function() {
      var input_str = "1. hello there how\n\n";
      var expected_arr = [["1."," hello there how"],["",""],["",""]];
      var res=OrchParser.preparse_array_of_strings("[0-9]\\.", input_str);
      var compare_res=compare_arrays(res, expected_arr);
      expect(compare_res).to.equal(true);
    });
    it("should handle not finding anything", function() {
      var input_str = "hello there how\nare you\ni am fine";
      var expected_arr = [["","hello there how"],["","are you"],["","i am fine"]];
      var res=OrchParser.preparse_array_of_strings("[0-9]\\.", input_str);
      var compare_res=compare_arrays(res, expected_arr);
      expect(compare_res).to.equal(true);
    });

    describe("find_item_details_for_sow_id", function(){
      it("", function() {
        // var ip_str="fodder\nhello\nblah(xxx)\tthere\nhow\nare\nanother(yyy)blah\nblah2";
var ip_str = "Firewall-Infrastructure-New-Complex (WS-C4999-E)  2   $     1,837.33 $   3,674.66 $  176,383.68\n Firewall-Support line 1\t2\t$     324.44    $   648.88  $  31,146.24\nFirewall-Support xline2\t2\t$     324.44  $   648.88  $  31,146.24\nFirewall-Infrastructure-New-Complex (ASA6666)\t2\t$    - $   -    $  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)\t2  $     1,837.33 $   3,674.66 $  176,383.68\n Firewall-Support YYYY\t2\t$     324.44  $   648.88  $  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)   $    470.97 $   1,883.88    $  90,426.24\n"

        var res=OrchParser.find_item_details_for_sow_id("WS-C4999-E", ip_str);
        var expected=["WS-C4999-E", ",  Firewall-Support line 1, Firewall-Support xline2"]
        var compare_res=compare_arrays(res, expected);
        console.log(res);
      });
    }); // find_item_details_for-sowid
  });

  // describe("#greets", function() {
  //   it("should throw if no target is passed in", function() {
  //     expect(function() {
  //       (new Cow()).greets();
  //     }).to.throw(Error);
  //   });

  //   it("should greet passed target", function() {
  //     var greetings = (new Cow("Kate")).greets("Baby");
  //     expect(greetings).to.equal("Kate greets Baby");
  //   });
  // });
});