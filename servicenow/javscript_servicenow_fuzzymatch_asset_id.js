#!/usr/local/bin/js

require('sn_orch_parser');


function print_out_sow_component_array(item_description_arr)
{
  for (i = 0; i < item_description_arr.length; i++) { 
    // s=i.toString() + ": " + arr[i].toString();
    debug(print_out_item_data(item_description_arr[i][0], item_description_arr[i][1]));
  }
}


// MAIN Start
var sow_bill_of_materials;
var detail_bom_info;

// simple example for one asset id
sow_bill_of_materials =  "1.0	WS-4999";
detail_bom_info = "Firewall-Infrastructure-New-Comple (Cat4999)\t2\t$1,837.33\t$   3,674.66\t$  176,383.68\n Firewall-Support line 1\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2\t$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4998-F)\t2\t$     1,837.33\t$   3,674.66\t$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44\t$   648.88\t$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)\t$    470.97\t$   1,883.88\t$  90,426.24\n"

// STEP 1
// var user_sow_filename = get_user_selection();

// STEP 2
// get text from file for user_sow_filename
// sow_bill_of_materials=run_mid(gen_get_data_exec(user_sow_filename, "Bill of Materials"))
// sow_quote_costs=run_mid(gen_get_data_exec(user_sow_filename, "Quote"))

// STEP 3
var SOW_ITEM_IDS=find_sow_ids_in_quote_costs(sow_bill_of_materials);
if (SOW_ITEM_IDS === null) { raise_error("No SOW Item IDs found"); }

// STEP 4
var SOW_ITEM_IDS_AND_DESCRIPTION=OrchParser(SOW_ITEM_IDS, sow_quote_costs);
if (SOW_ITEM_IDS_AND_DESCRIPTION.length <= 0 ) { raise_error("No SOW Item Descriptions Found"); }

// STEP 5
// display result to user
print_out_sow_component_array(SOW_ITEM_IDS_AND_DESCRIPTION);

