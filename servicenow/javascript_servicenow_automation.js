#!/usr/local/bin/js

// this is the user entered 'value' e.g. the initial file number
// TODO get this from the user
var user_file_number;

var SOWSERVER="https://blab.blah.com"
var SOWDIR="FOLDER1/FOLDER2"
var absolute_url=SOWSERVER+"/"+SOWDIR
var get_data_script="get_data.exe"
var power_shell_script_exec_str="";

// when "get_data.exe" -test called automatically returns stubbed data based on filename and heading
var test_flag=" -test";


function run_mid(execstr)
{
  //TODO service now construct
}

// get data from Sharepoint means running .net program (name held in get_data_script)
// assumes script is stored in get_data_script
// get_data.exe <url/file> <heading>
function gen_get_data_exec(file, heading)
{
	if ( (file == NULL) || (file.length <= 0) ) return null;
	if ( (heading == NULL) || (heading.length <= 0) ) return null;
   var s = get_data_script + " " + file + " " + heading + test_flag;
   return s;	
}

function print_out_sow_component_array(item_description_arr)
{
  for (i = 0; i < item_description_arr.length; i++) { 
  	// s=i.toString() + ": " + arr[i].toString();
  	debug(print_out_item_data(item_description_arr[i][0], item_description_arr[i][1]));
  }
}

// creates xml structure for item data
//<item><asset_id>xxxx</asset_id><components>blah blah</components>
function print_out_item_data(sow_asset_id, components_text)
{
	x="<item><asset_id>" + sow_asset_id + "</asset_id><components>" + components_text + "</components><item>"
	return x;
}

// TODO make this service now to show user error
function raise_error(e) {
	throw "Error: " + e;
}


//
// REGEX code to do automation/matching
//

// look for all occurances of 1.0\sow asset id\t
// and return the 2nd array element = sow asset id
function find_sow_ids(bom_str) {
	var matches;
	var allmatches = [];
	// matches=bom_str.match(/([1-9][0-9]*\.0\t([A-Z][A-Za-z0-9\-]*)\t)+/);
	var lines = bom_str.split('\n');
		for(var i = 0;i < lines.length;i++){
			// debug("line counter: " + i.toString());
	      matches=lines[i].match(/[1-9][0-9]*\.0\t([A-Z][A-Za-z0-9\-]*)\t+/);
	      if (matches === null) { continue; }
			// debug("find_sow_ids matches: " + matches[1].toString());
		    //code here using lines[i] which will give you each line
		   allmatches.push(matches[1]);
		}
	// debug(allmatches.length.toString());
	// debug("all matches:");
	// debug(allmatches.toString());
	return allmatches;
}

// for the id, find "1.0 id" from description_text and tet all text after on the same line
// TODO precomplie the description text so we have items + components parsed only once
// TODO handle when there are more than one top component with the id listed
function regExpEscape(literal_string) {
    return literal_string.replace(/[-[\]{}()*+!<=:?.\/\\^$|#\s,]/g, '\\$&');
}
function find_item_details_for_sow_id(id, description_text) {
	var matches;
	var desc_minus_numbers;
	var regExp_headers;
   var matches_of_description;
   // array [0] = asset id [1] = is all coponents that are part of that asset in single line comma separated
   var all_content_for_asset_id = [];
   var component_info = "";
   var in_block;

   // debug("loking at description for " + id.toString());
   // debug(description_text);


   var lines = description_text.split('\n');
   in_block=false;
	for(var i = 0;i < lines.length;i++){
		theline=lines[i];
		// have we found the top level item e.g.  description blah (asset id) qty price etc....
		reg='(.*)\\(' + regExpEscape(id) + '\\).*';
      matches=theline.match(reg);

      // yes so record this asset, now mark the fact we keep going
       if (matches !== null) {
      	in_block = true;
      	all_content_for_asset_id[0] = id;
      	continue; }

      // get all stuff before price / qty / etc.
      if (in_block) {
			reg='([ A-Za-z\t0-9\-]+)\t[0-9]+';
			// reg='(.*)\t(.*)';
	      matches=theline.match(reg);
	      if (matches !== null) {
	      	component_info = component_info + ", " + matches[1];
	      	continue;
	      }
      	// test for the next top line i.e. an asset descripton with (xxxxxx) in the line
         matches=theline.match(/.*\([A-Za-z0-9\-]+\).*/)
         // we have now found the next top line item so finish this search
         if (matches !== null) { 
         	in_block=false;
         	break;
         }
      }
	 } // for
            all_content_for_asset_id[1]=component_info;
	return all_content_for_asset_id
} // find_item_details_for_sow_id

// for each SOW ID we need to find where it ocurs in the detail_bom_info
// now match 1.0, 11.0, 2.0 etc. and text after it until tab
// e.g. 1.0 AS-JASDF\tblah blahblah
// assumes
// new line before 1.0 XXXX 
// space between 1.0 AND XXXX
// text up until cell
// TODO what other characters in the regex header
function lookup_sow_id_description(list_of_sows, detail_info) {
	// debug("list of sows: " + list_of_sows.toString());
	var ret_array=[];
	var description;
	for (i = 0; i < list_of_sows.length; i++) { 
	   si=list_of_sows[i];
	   // debug("lookup sow id: si: "  + si);
	   description=find_item_details_for_sow_id(si, detail_info);

	   if (description === null) continue;

	   // add recorded entry to our list
	   ret_array.push([si, description]);
	}
	return ret_array;
} //lookup_sow_id_description




// test data
// override these variables to see outcome
// e.g. 
// $ jsc javascript_servicenow_automation.js 
// --> <item><asset_id>WX-C9999-X</asset_id><components>WX-C9999-X,,  Firewall-Support line 1, Firewall-Support xline2</components><item>
// --> <item><asset_id>WS-c4999-F</asset_id><components>WS-c4999-F,,  Firewall-Support YYYY</components><item>


// MAIN Start
var sow_bill_of_materials;
var detail_bom_info;

// // example with 2 asset ids
sow_bill_of_materials =  "1.0	WX-C9999-X	Cat4500 E-Series 6-Slot Chassis fan no ps	2	$7,461.75 \n1.1 WS-X4748-RJ45-E	Catalyst 4500 E-Series 48-Port10/100/1000 Non-Bl;ocking	2	$10,449.44\n2.0	WS-c4999-F	Cat4500 E-Series 6-Slot Chassis fan no ps	2	$7,461.75\n2.1	WS-X4748-RJ45-E	Cataqlyst 4500 E-Series 48-Port 10/100/1000 Non-Blcoking	2	$7,461.75";
// use this example with abgove sow_bill_of_materails and the components will be empoty
// detail_bom_info = "lakdsjflkadjsf lkadjsfladsj flkj afdslkj adslkfj aldfskj fa";

 detail_bom_info = "Pricing Table Notes: \n \n 1.	A Contract Variation will be executed between the parties to add the incremental charge to the existing ‘TWS Service 3 – Data Centre to Data Centre’ Resource Unit\n 2.	This solution will be delivered under the T&Cs of the existing DNV agreement between Qantas and Telstra. A contract variation will be required to add some new Resource Units (price points), otherwise the service model will be as per the existing agreement\n \n 22\n \n Once Off Charges\n \n \n Consultancy Services ~ GST Excl	Units	Unit Price	Extended Price\n \n \n Proramme Support / Imlementation Da - 8 hours\n 5	1135.68	$5,678.40\n \n Ongoing Resource Unit Charges\n Additional Resource Units -GST Excl	Quanity	RU Price(per month)	Unit Extended Price	Total Contract Value\n Firewall-Infrastructure-New-Complex (WX-C9999-X)	2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support line 1\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2\t$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)\t2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44	$   648.88	$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)	$    470.97 $   1,883.88	$  90,426.24\n"

// simple example for one asset id
 // sow_bill_of_materials =  "1.0	WS-c4999-F	Cat4500 ";
// detail_bom_info = "Firewall-Infrastructure-New-Complex (WX-C9999-X)	2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support line 1\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2\t$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4999-F)\t2	$     1,837.33 $   3,674.66	$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44	$   648.88	$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)	$    470.97 $   1,883.88	$  90,426.24\n"
// detail_bom_info = "blah\nblah\n1.0 XXXXXX\thello there after XXXX\tcell2\tcell3\n something else\n something else\n2.0 xxxxxxx nothing here\nblah\nblah\n3.0 YYYYYY\tthis is text after YYYYYY\tcell2.x\n";
// detail_bom_info = "Firewall-Infrastructure-New-Complex (YYYYYY) blah	2	$     1,837.33 $   3,674.66	$  176,383.68"

// with no matches
//detail_bom_info = "blah\nblah\n1.0 X\thello there after XXXX\tcell2\tcell3\n something else\n something else\n2.0 xxxxxxx nothing here\nblah\nblah\n3.0 Y\tthis is text after YYYYYY\tcell2.x\n";





// STEP 1
// var user_sow_filename = get_user_selection();

// STEP 2
// get text from file for user_sow_filename
// sow_bill_of_materials=run_mid(gen_get_data_exec(user_sow_filename, "Bill of Materials"))
// detail_bom_info=run_mid(gen_get_data_exec(user_sow_filename, "Quote"))

// STEP 3
var SOW_ITEM_IDS=find_sow_ids(sow_bill_of_materials);
if (SOW_ITEM_IDS === null) { raise_error("No SOW Item IDs found"); }

// STEP 4
var SOW_ITEM_IDS_AND_DESCRIPTION=lookup_sow_id_description(SOW_ITEM_IDS, detail_bom_info);
if (SOW_ITEM_IDS_AND_DESCRIPTION.length <= 0 ) { raise_error("No SOW Item Descriptions Found"); }

// STEP 5
// display result to user
print_out_sow_component_array(SOW_ITEM_IDS_AND_DESCRIPTION);



