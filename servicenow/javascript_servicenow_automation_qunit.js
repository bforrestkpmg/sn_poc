#!/usr/local/bin/js


// tests to do 

// function run_mid(execstr)
// function print_out_array(arr)
// function raise_error(e) {


// function gen_get_data_exec(file, heading)
// gen_get_data_exec("https://blah.com/hello/there", "this heading") => "exec.exe https://blah.com/hello/there "this heading"
// gen_get_data_exec("https://blah.com/hello/there", "") => null

// with no matches
//detail_bom_info = "blah\nblah\n1.0 X\thello there after XXXX\tcell2\tcell3\n something else\n something else\n2.0 xxxxxxx nothing here\nblah\nblah\n3.0 Y\tthis is text after YYYYYY\tcell2.x\n";

// function gen_get_data_exec(file, heading)

// function clean_up_sow_item_ids(arr)
// (xxxxx),(yyyyyy) => xxxxx,yyyyyy
// xxxxx,(yyyyyy) => xxxxx,yyyyyy
// [] => []

// function find_sow_ids(bom_str)
//"hello there (XXXXXX), how are you doing (YYYYYY) blah blah" => (XXXXXX),(YYYYYY)
//"hello there (), how are you doing (YYYYYY) blah blah" => (YYYYYY)
//"hello there (129xx81-1), how are you doing (YYYYYY) blah blah" => ((129xx81-1),(YYYYYY)

// function find_item_details_for_sow_id(id, description_text)
// "blah\nblah\n1.0 XXXXXX\thello there after XXXX\tcell2\tcell3\n something else\n something else\n2.0 xxxxxxx nothing here\nblah\nblah\n3.0 YYYYYY\tthis is text after YYYYYY\tcell2.x\n"; => 
// XXXXXX => hello there after XXXX  cell2 cell3,XXXXXX, cell3
// "blah\nblah\n1.0\thello there after XXXX\tcell2\tcell3\n something else\n something else\n2.0 xxxxxxx nothing here\nblah\nblah\n3.0 YYYYYY\tthis is text after YYYYYY\tcell2.x\n"; => 
// XXXXXX => null


// detail_bom_info = "blah\nblah\n1.0 XXXXXX\thello there after XXXX\tcell2\tcell3\n something else\n something else\n2.0 xxxxxxx nothing here\nblah\nblah\n3.0 YYYYYY\tthis is text after YYYYYY\tcell2.x\n";
// function lookup_sow_id_description(["XXXXXX", "YYYYYY"], detail_info) => 
// XXXXXX,  hello there after XXXX  cell2 cell3,XXXXXX, cell3
// YYYYYY,  this is text after YYYYYY cell2,YYYYYY, cell2
