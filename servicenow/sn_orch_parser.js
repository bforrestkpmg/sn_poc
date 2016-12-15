// cow.js
(function(exports) {
  "use strict";

  function OrchParser(asset_id) {
    this.asset_id = asset_id || null;
    this.closest_asset_id = null;
    this.distance = null;
  }
  exports.OrchParser= OrchParser;

  OrchParser.prototype = {
    preparse_for_fuzzy: function(s) {
        var t="";
        t=s.replace(/[\[\]-]/g, '');
        return t;
    }
  };
})(this);

