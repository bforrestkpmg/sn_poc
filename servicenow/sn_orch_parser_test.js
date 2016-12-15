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
      expect(OrchParser.preparse_array_of_strings("x", "")).to.equal("");
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