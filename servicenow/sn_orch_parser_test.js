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
      var expected_arr = [["1.","hello there how"],["","2are you"],["3.","i am fine"]];
      var res=OrchParser.preparse_array_of_strings("[0-9]", input_str);
      var compare_res=compare_arrays(res, expected_arr);
      expect(compare_res).to.equal(true);
    });
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