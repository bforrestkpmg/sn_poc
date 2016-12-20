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
describe("levenshtein", function() {
	describe("get", function() {
		// these are extracted from fast-levenshtein tests for now
		it("matches distance", function() {
			res=OrchParser.get_levi("a", "a");
			expect(res).to.equal(0);
			res=OrchParser.get_levi("xabxcdxxefxgx", "1ab2cd34ef5g6")
			expect(res).to.equal(6);
			res=OrchParser.get_levi("distance", "difference")
			expect(res).to.equal(5);

					});
					});
					});

describe("OrchParser", function() {
	describe("support functions", function() {
		it("splits strings ", function() {
		var input_str = "hello there how";
		var expected_arr = [ "hello", "there", "how" ];
		var res=OrchParser.split_components_further(input_str, " ");
		console.log(res);
		var compare_res=compare_arrays(res, expected_arr);
		expect(compare_res).to.equal(true);
	});
	});

	describe("constructor", function() {
		it("defaults and sets asset id", function() {
			OrchParser.setup();
		});

		it("should preparse asset ids by stripping out characters", function() {
			expect(OrchParser.preparse_asset_id("hellow-][there")).to.equal("hellowthere");
		});
		it("should create array of preparsed content", function() {
			var input_str = "before1 1. hello there how\n2are you\nbefore3 3.i am fine";
			var expected_arr = [[ "1.", "before1 "," hello there how"], ["", "2are you", ""], ["3.", "before3 ", "i am fine"]]
			var res=OrchParser.preparse_array_of_strings("[0-9]\\.", input_str);
			var compare_res=compare_arrays(res, expected_arr);
			expect(compare_res).to.equal(true);
		});
		it("should handle empty strings", function() {
			var input_str = "1. hello there how\n\n";
			var expected_arr = [["1.", "", " hello there how"], ["", "", ""], ["", "", ""]];
			var res=OrchParser.preparse_array_of_strings("[0-9]\\.", input_str);
			var compare_res=compare_arrays(res, expected_arr);
			expect(compare_res).to.equal(true);
		});
		it("should handle not finding anything", function() {
			var input_str = "hello there how\nare you\ni am fine";
			var expected_arr = [["", "hello there how", ""], ["", "are you", ""], ["", "i am fine", ""]];
			var res=OrchParser.preparse_array_of_strings("[0-9]\\.", input_str);
			var compare_res=compare_arrays(res, expected_arr);
			expect(compare_res).to.equal(true);
		});

		describe("get asset ids", function(){
			it("should extract asset ids prefixed with n.0 top line items", function() {
				// var ip_str="fodder\nhello\nblah(xxx)	there\nhow\nare\nanother(yyy)blah\nblah2";
			var ip_str = "1.0	WX-C9999-E	Cat4500 E-Series 6-Slot Chassis fan no ps	2	$7,461.75 \n1.1	WS-X4748-RJ45-E	Catalyst 4500 E-Series 48-Port10/100/1000 Non-Bl;ocking	2	$10,449.44\n2.0	WS-c4999-F	Cat4500 E-Series 6-Slot Chassis fan no ps	2	$7,461.75\n2.1	WS-X4748-RJ45-E Cataqlyst 4500 E-Series 48-Port 10/100/1000 Non-Blcoking	2	$7,461.75"; 
				var res=OrchParser.find_sow_ids_in_quote_costs(ip_str);
				var expected=["WX-C9999-E", "WS-c4999-F"];
				var compare_res=compare_arrays(res, expected);
				expect(compare_res).to.equal(true);
			});
			it("should return empty if no brackets", function() {
				// var ip_str="fodder\nhello\nblah(xxx)	there\nhow\nare\nanother(yyy)blah\nblah2";
			var ip_str = "WX-C9999-E	Cat4500 E-Series 6-Slot Chassis fan no ps	2	$7,461.75 \n1.1	WS-X4748-RJ45-E	Catalyst 4500 E-Series 48-Port10/100/1000 Non-Bl;ocking	2	$10,449.44\nWS-c4999-F	Cat4500 E-Series 6-Slot Chassis fan no ps	2	$7,461.75\n2.1	WS-X4748-RJ45-E Cataqlyst 4500 E-Series 48-Port 10/100/1000 Non-Blcoking	2	$7,461.75"; 
				var res=OrchParser.find_sow_ids_in_quote_costs(ip_str);
				var expected=[];
				var compare_res=compare_arrays(res, expected);
				expect(compare_res).to.equal(true);
			});
		}); // find_item_details_for-sowid
		describe("get components for asset ids", function(){
				it("generates list of closest", function() {

			var input_arry = [[ "1.", "before1 "," hello there how"], ["", "2are you", ""], ["3.", "before3 ", "i am fine"]];
			var res=OrchParser.find_closest_or_exact_match("3.", input_arry);
			expect(res).to.equal(2);
			});
			it("generates list of closest when not precisely the same", function() {

			var input_arry = [[ "WX-C9999-E", "before1 "," hello there how"], ["", "2are you", ""], ["WX-C7777-E", "before3 ", "i am fine"]];
			var res=OrchParser.find_closest_or_exact_match("cat7777", input_arry);
			expect(res).to.equal(2);
			});
			it("generates list of closest when similar", function() {

			var input_arry = [[ "WX-C9999-E", "before1 "," hello there how"], ["", "2are you", ""], ["WX-C7777-E", "before3 ", "i am fine"]];
			var res=OrchParser.find_closest_or_exact_match("CAT7777", input_arry, 5);
			expect(res).to.equal(2);
			});

			it("finds components in brackets below the current top line item for a specific id", function() {
			var ip_arr = [
			["","somecontent",""],
			["(WX-C9999-E)","Firewall - Infrastructure - New - Complex","after"],
			["","component 1",""],
			["","component 2",""],
			["(WX-C88888-E)","blah before","blah after"],
			["","blah2 before","blah2 after"]
			];
				var res=OrchParser.find_item_details_for_sow_id(1, ip_arr);
				var expected=["(WX-C9999-E)", "Firewall - Infrastructure - New - Complex after, component 1, component 2, " ];
				var compare_res=compare_arrays(res, expected);
				expect(compare_res).to.equal(true);
			});
		it("finds components in brackets handling only 1 set of components", function() {
			var ip_arr = [
			["","somecontent",""],
			["(WX-C9999-E)","Firewall - Infrastructure - New - Complex","after"],
			["","component 1",""],
			["","component 2",""]
			];
				var res=OrchParser.find_item_details_for_sow_id(1, ip_arr);
				var expected=["(WX-C9999-E)","Firewall - Infrastructure - New - Complex after, component 1, component 2, "];
				var compare_res=compare_arrays(res, expected);
				expect(compare_res).to.equal(true);
			});
				it("finds components in brackets handling no components", function() {
			var ip_arr = [
			["(WX-C9999-E)","Firewall - Infrastructure - New - Complex","after"]
			];
				var res=OrchParser.find_item_details_for_sow_id(0, ip_arr);
				var expected=["(WX-C9999-E)", "Firewall - Infrastructure - New - Complex after, " ];
				var compare_res=compare_arrays(res, expected);
				expect(compare_res).to.equal(true);
			});

			});
			describe("get all assets and all components", function(){
					it("for a list of ids, get the components for each", function() {
			var sow_quote_costs = "Pngoing Resource Unit Charges\n Additional Resource Units -GST Excl	Quanity	RU Price(per month)	Unit Extended Price	Total Contract Value\n Firewall-Infrastructure-New-Complex (WX-C9999-X)\t2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support line 1\t2	$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline\t2	2	$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2	$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)\t2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44	$   648.88	$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)\t$    470.97 $   1,883.88	$  90,426.24\n"; 
				// var res=OrchParser.get_sow_asset_ids_description_from_bom(["WX-C9999-X", "WS-c4999-F"], sow_quote_costs);
				var res=OrchParser.get_sow_asset_ids_description_from_bom(["WX-C9999-X"], sow_quote_costs);
				console.log(res);
				var expected=[["WX-C9999-X", ",  Firewall-Support line 1, Firewall-Support xline2"], ["WS-c4999-F", ",  Firewall-Support YYYY"]];
				var compare_res=compare_arrays(res, expected);
				expect(compare_res).to.equal(true);
			});
			});
		describe("fuzzy match asset ids", function(){
				it("builds a table of likely matches", function() {
				var ip_array=[["(12x)", " there how"], ["(xxy)", "ok"], ["(xxxxxaa)", "thankyou"]];
			     var res=OrchParser.calc_fuzzy_match_to_regex_list("xxx", ip_array);
			     var expected_arr=[["(12x)", 4, " there how"], ["(xxy)", 3, "ok"], ["(xxxxxaa)", 6, "thankyou"]];
				var compare_res=compare_arrays(res, expected_arr);
				expect(compare_res).to.equal(true);
				});
		it("finds next best serach item match based on nearest (based on shortest levenshtein distance that is not zero)", function() {
			     var ip_array=[["(12x)", 4, " there how"], ["(xxy)", 3, "ok"], ["(xxxxxaa)", 6, "thankyou"]];
			     var res=OrchParser.get_closest_match_from_fuzzy_match_list(ip_array);
				expect(res).to.equal("(xxy)");
			});
			it("handles when nothing found", function() {
var ip_array=[["(12x)", " there how"], ["(xxy)", "ok"], ["(xxxxxaa)", "thankyou"]];
			     var res=OrchParser.calc_fuzzy_match_to_regex_list("xxx", ip_array);
			     var expected_arr=[["(12x)", 4, " there how"], ["(xxy)", 3, "ok"], ["(xxxxxaa)", 6, "thankyou"]];
				var compare_res=compare_arrays(res, expected_arr);
				expect(compare_res).to.equal(true);
			});
			it	("finds actual match too)", function() {
					var ip_array=[["(12x)", " there how"], ["(xxx)", "ok"], ["(xxxxxaa)", "thankyou"]];
					  var res_arr=OrchParser.calc_fuzzy_match_to_regex_list("xxx", ip_array);
			     var res=OrchParser.get_closest_match_from_fuzzy_match_list(res_arr);
				expect(res).to.equal("(xxx)");
			});
			
			});
	});
});