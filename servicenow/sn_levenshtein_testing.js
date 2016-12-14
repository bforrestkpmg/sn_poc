// If expected and actual are the equivalent, pass the test.
function assertEquivalent(msg, expected, actual) {
  if (typeof actual == 'undefined') {
    // msg is optional.
    actual = expected;
    expected = msg;
    msg = 'Expected: \'' + expected + '\' Actual: \'' + actual + '\'';
  }
  if (_equivalent(expected, actual)) {
    assertEquals(msg, String.toString(expected), String.toString(actual));
  } else {
    assertEquals(msg, expected, actual);
  }
}


// Are a and b the equivalent? -- Recursive.
function _equivalent(a, b) {
  if (a == b) {
    return true;
  }
  if (typeof a == 'object' && typeof b == 'object' && a !== null && b !== null) {
    if (a.toString() != b.toString()) {
      return false;
    }
    for (var p in a) {
      if (!_equivalent(a[p], b[p])) {
        return false;
      }
    }
    for (var p in b) {
      if (!_equivalent(a[p], b[p])) {
        return false;
      }
    }
    return true;
  }
  return false;
}


function diff_rebuildtexts(diffs) {
  // Construct the two texts which made up the diff originally.
  var text1 = '';
  var text2 = '';
  for (var x = 0; x < diffs.length; x++) {
    if (diffs[x][0] != DIFF_INSERT) {
      text1 += diffs[x][1];
    }
    if (diffs[x][0] != DIFF_DELETE) {
      text2 += diffs[x][1];
    }
  }
  return [text1, text2];
}



// DIFF TEST FUNCTIONS

levenshtein = require('./levenshtein');


function testBasicLevin1() {
  ret=levenshtein.get("a", "a", 1);
  document.write(ret.toString());
  return ("hello");
}


function testPatchApply() {
  dmp.Match_Distance = 1000;
  dmp.Match_Threshold = 0.5;
  dmp.Patch_DeleteThreshold = 0.5;

  // Exact match.
  var patches = dmp.patch_make('The quick brown fox jumps over the lazy dog.', 'That quick brown fox jumped over a lazy dog.');
  var results = dmp.patch_apply(patches, 'The quick brown fox jumps over the lazy dog.');
  assertEquivalent(['That quick brown fox jumped over a lazy dog.', [true, true]], results);

  // Partial match.
  results = dmp.patch_apply(patches, 'The quick red rabbit jumps over the tired tiger.');
  assertEquivalent(['That quick red rabbit jumped over a tired tiger.', [true, true]], results);

  // Failed match.
  results = dmp.patch_apply(patches, 'I am the very model of a modern major general.');
  assertEquivalent(['I am the very model of a modern major general.', [false, false]], results);

  // Big delete, small change.
  patches = dmp.patch_make('x1234567890123456789012345678901234567890123456789012345678901234567890y', 'xabcy');
  results = dmp.patch_apply(patches, 'x123456789012345678901234567890-----++++++++++-----123456789012345678901234567890y');
  assertEquivalent(['xabcy', [true, true]], results);

  
}


