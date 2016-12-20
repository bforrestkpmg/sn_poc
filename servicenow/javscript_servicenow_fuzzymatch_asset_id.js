#!/usr/local/bin/js

require 'sn_orch_parser';



// MAIN Start
var sow_bill_of_materials;
var detail_bom_info;

// simple example for one asset id
sow_bill_of_materials =  "1.0	WS-4999";
detail_bom_info = "Firewall-Infrastructure-New-Comple (Cat4999)\t2\t$1,837.33\t$   3,674.66\t$  176,383.68\n Firewall-Support line 1\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Support xline2\t2\t$     324.44	$   648.88	$  31,146.24\nFirewall-Infrastructure-New-Complex (ASA5585)\t2\t$    - $   -	$  -\n Firewall-Infrastructure-New-Complex (WS-c4998-F)\t2\t$     1,837.33\t$   3,674.66\t$  176,383.68\n Firewall-Support YYYY\t2\t$     324.44\t$   648.88\t$  31,146.24\n Firewall-Infrastructure-New-Blah (Cat4506)\t$    470.97\t$   1,883.88\t$  90,426.24\n"

	
