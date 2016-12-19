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
	describe("constructor", function() {
		it("defaults and sets asset id", function() {
			OrchParser.setup();
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
					it("finds components in brackets below the current top line item for a specific id", function() {
			var ip_str = "textbefore(WX-C9999-E)	2	$1,837.33	$	3,674.66	$  176,383.68\nwsc4999 first line compnoent	2	$ 324.44	$ 648.88	$  31,146.24\nwsc4999 2nd line	2	$324.44	$648.88	$31,146.24\nnew item(ASA6666)	2	$-	$-	$ -\n new item 4d line(WS-c4999-F)	2	$1,837.33	$3,674.66	$  176,383.68\nwsc49999f 2ndline	2	$324.44	$648.88	$31,146.24\nnothing to see here	9	$ 470.97 	$   1,883.88	$  90,426.24\n" 
				var res=OrchParser.find_item_details_for_sow_id("WX-C9999-E", ip_str, true);
				var expected=["WX-C9999-E", ", wsc4999 first line compnoent, wsc4999 2nd line"];
				var compare_res=compare_arrays(res, expected);
				expect(compare_res).to.equal(true);
			});
			});
			// describe("get all assets and all components", function(){
			// 		it("for a list of ids, get the components for each", function() {
			// var sow_quote_costs = "Pricing Table Notes: \n \n 1.	A Contract Variation will be executed between the parties to add the incremental charge to the existing ‘TWS Service 3 – Data Centre to Data Centre’ Resource Unit\n 2.	This solution will be delivered under the T&Cs of the existing DNV agreement between Qantas and Telstra. A contract variation will be required to add some new Resource Units (price points), otherwise the service model will be as per the existing agreement\n \n 22\n \n Once Off Charges\n \n \n Consultancy Services ~ GST Excl	Units	Unit Price	Extended Price\n \n \n Proramme Support / Imlementation Da - 8 hours\n 5	1135.68	$5,678.40\n \n Ongoing Resource Unit Charges\n Additional Resource Units -GST Excl	Quanity	RU Price(per month)	Unit Extended Price	Total Contract Value\n Firewall-Infrastructure-New-Complex (WX-C9999-X)	2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support line 1\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2\t$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)\t2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44	$   648.88	$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)	$    470.97 $   1,883.88	$  90,426.24\n"; 
			// 	var res=OrchParser.get_sow_asset_ids_description_from_bom(["WX-C9999-X", "WS-c4999-F"], sow_quote_costs);
			// 	console.log(res);
			// 	var expected=[["WX-C9999-X", ",  Firewall-Support line 1, Firewall-Support xline2"], ["WS-c4999-F", ",  Firewall-Support YYYY"]];
			// 	var compare_res=compare_arrays(res, expected);
			// 	expect(compare_res).to.equal(true);
			// });
			// });
		describe("fuzzy match asset ids", function(){
				// it("builds a table of likely matches", function() {
				// 	var input_str = "";
				// 	var expected_arr = [];
				// 	var res=OrchParser.preparse_array_of_strings("[0-9]\\.", input_str);
				// 	var compare_res=compare_arrays(res, expected_arr);
				// 	expect(compare_res).to.equal(true);
				// });
		it("finds next best components based on nearest (based on shortest levenshtein distance that is not zero)", function() {
			var ip_str= "Firewall-Infrastructure-New-Complex (Cat4506)	2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support forr cat4506 line 1\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2\t$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (AA-D-4506)\t2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44	$   648.88	$  31,146.24\n"; 
				var res=OrchParser.wrapper_find_item_details_for_sow_id("WS-C4506", ip_str);
				var expected=["WS-C4506", " Firewall-Support forr cat4506 line 1", "fuzzy:Cat4506"];
				var compare_res=compare_arrays(res, expected);
				expect(compare_res).to.equal(true);
			});
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