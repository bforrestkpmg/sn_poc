#!/usr/local/bin/js

// this is the user entered 'value' e.g. the initial file number
// TODO get this from the user
var user_file_number;

var SOWSERVER="https://blab.blah.com"
var SOWDIR="FOLDER1/FOLDER2"
var absolute_url=SOWSERVER+"/"+SOWDIR
var get_data_script="get_data.exe"
var power_shell_script_exec_str="";


function run_mid(execstr)
{
  //TODO figure out how to do this	in service now
}

// get data from SOWSERVER
// assumes script is 
// get_data.exe <url/file> <heading>
function gen_get_data_exec(file, heading)
{
	if ( (file == NULL) || (file.length <= 0) ) return null;
	if ( (heading == NULL) || (heading.length <= 0) ) return null;
   var s = get_data_script + " " + file + " " + heading;
   return s;	
}


// loop through all entries and remove parenthesis
function clean_up_sow_item_ids(arr)
{
  var newarr=[];
  for (i = 0; i < arr.length; i++) { 
  	s=arr[i].replace(/[\(\)]/g, "");
  	newarr.push(s)
  }
  return newarr;
}

function print_out_array(arr)
{
  for (i = 0; i < arr.length; i++) { 
  	s=i.toString() + ": " + arr[i].toString();
  	 debug(s);
  }
}

// TODO make this service now 'function'
function raise_error(e) {
	throw "Error: " + e;
}

// look for all occurances of 1.0\sow asset id\t
// and return the 2nd array element = sow asset id
function find_sow_ids(bom_str) {
	var matches;
	var allmatches = [];
	// matches=bom_str.match(/([1-9][0-9]*\.0\t([A-Z][A-Za-z0-9\-]*)\t)+/);
	var lines = bom_str.split('\n');
		for(var i = 0;i < lines.length;i++){
			debug("line counter: " + i.toString());
	      matches=lines[i].match(/[1-9][0-9]*\.0\t([A-Z][A-Za-z0-9\-]*)\t+/);
	      if (matches === null) { continue; }
			debug("matches: " + matches[1].toString());
		    //code here using lines[i] which will give you each line
		   allmatches=allmatches+matches[1];
		}


	// no cleanup for now
	// if (matches !== null) {
	// 	matches=clean_up_sow_item_ids(matches);
	// }
	debug(allmatches.length.toString());
	debug(allmatches.toString());
	return allmatches;
}

// for the id, find "1.0 id" from description_text and tet all text after on the same line
function find_item_details_for_sow_id(id, description_text) {
	var matches;
	var desc_minus_numbers;
	var regExp_headers;
   var matches_of_description;

	 // find (sowid) and text after e.g. 1.0 ASX-asdfkj2 blah blah lbha
   regExp_headers = new RegExp('\\n[1-9][0-9]*\.[0-9] '+ id +'(\\t[A-Za-z0-9 \,]+)*')
   matches_of_description = regExp_headers.exec(description_text);

   if (matches_of_description !== null)
   {
	   //strip out the preceeding 1.0, 2.0  and keep rest of the dtails of this line
	   reg = new RegExp('\\n[1-9][0-9]*\.[0-9] '+ si , 'g');
	   desc_minus_numbers=matches_of_description.toString().replace(reg,"");
		return desc_minus_numbers;
	}
	return null;
}

// for each SOW ID we need to find where it ocurs in the detail_bom_info
// now match 1.0, 11.0, 2.0 etc. and text after it until tab
// e.g. 1.0 AS-JASDF\tblah blahblah
// assumes
// new line before 1.0 XXXX 
// space between 1.0 AND XXXX
// text up until cell
// TODO what other characters in the regex header
function lookup_sow_id_description(list_of_sows, detail_info) {
	var ret_array=[];
	var description;
	for (i = 0; i < list_of_sows.length; i++) { 
	   si=list_of_sows[i];
	   description=find_item_details_for_sow_id(si, detail_info);

	   if (description === null) continue;

	   // add recorded entry to our list
	   ret_array.push([si, description]);
	}
	return ret_array;
} //lookup_sow_id_description


// MAIN Start

// this needs to be retrieved from
var sow_bill_of_materials;
var detail_bom_info;

// test data
// sow_bill_of_materials =  "hello there (XXXXXX), how are you doing (YYYYYY) blah blah";
sow_bill_of_materials =  "1.0	WS-C4506-E	Cat4500 E-Series 6-Slot Chassis fan no ps	2	$7,461.75 \n1.1 WS-X4748-RJ45-E	Catalyst 4500 E-Series 48-Port10/100/1000 Non-Bl;ocking	2	$10,449.44\n2.0	WS-c4999-F	Cat4500 E-Series 6-Slot Chassis fan no ps	2	$7,461.75\n2.1	WS-X4748-RJ45-E	Cataqlyst 4500 E-Series 48-Port 10/100/1000 Non-Blcoking	2	$7,461.75";

detail_bom_info = "blah\nblah\n1.0 XXXXXX\thello there after XXXX\tcell2\tcell3\n something else\n something else\n2.0 xxxxxxx nothing here\nblah\nblah\n3.0 YYYYYY\tthis is text after YYYYYY\tcell2.x\n";
//detail_bom_info = "Firewall-Infrastructure-New-Complex (YYYYYY) blah	2	$     1,837.33 $   3,674.66	$  176,383.68"

// with no matches
//detail_bom_info = "blah\nblah\n1.0 X\thello there after XXXX\tcell2\tcell3\n something else\n something else\n2.0 xxxxxxx nothing here\nblah\nblah\n3.0 Y\tthis is text after YYYYYY\tcell2.x\n";

// sow_bill_of_materials=run_mid(gen_get_data_exec(absolute_url+user_file_number, "Bill of Materials"))
// detail_bom_info=run_mid(gen_get_data_exec(absolute_url+user_file_number, "Appendix 1:XXXX"))

// STEP 1
var SOW_ITEM_IDS=find_sow_ids(sow_bill_of_materials);
if (SOW_ITEM_IDS === null) { raise_error("No SOW Item IDs found"); }

// STEP 2
var SOW_ITEM_IDS_AND_DESCRIPTION=lookup_sow_id_description(SOW_ITEM_IDS, detail_bom_info);
if (SOW_ITEM_IDS_AND_DESCRIPTION.length <= 0 ) { raise_error("No SOW Item Descriptions Found"); }

print_out_array(SOW_ITEM_IDS_AND_DESCRIPTION);



