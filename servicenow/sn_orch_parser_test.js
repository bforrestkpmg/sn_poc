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
      var op = new OrchParser("blah");
      expect(op.asset_id).to.equal("blah");
      expect(op.closest_asset_id).to.equal(null);
      expect(op.distance).to.equal(null);
    });

    it("should preparse asset ids by stripping out characters", function() {
      var op = new OrchParser("blah");
      expect(op.preparse_for_fuzzy("hellow-][there")).to.equal("hellowthere");
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