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
    preparse_for_fuzzy: function(target) {
      if (!target)
        throw new Error("missing target");
      return this.name + " greets " + target;
    }
  };
})(this);