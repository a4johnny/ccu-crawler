var guestbook = /** @class */ (function () {
    function guestbook() {
    }
    return guestbook;
}());
var guestbooklistModel = /** @class */ (function () {
    function guestbooklistModel() {
        this.Guestbooklist = [];
    }
    return guestbooklistModel;
}());
var guestbooklistView = /** @class */ (function () {
    function guestbooklistView(m) {
        this.Model = m;
        this.CreateView();
        this.Update();
    }
    guestbooklistView.prototype.CreateView = function () {
    };
    guestbooklistView.prototype.Update = function () {
        //   console.log(e[0].Score);
        this.Model.Guestbooklist.forEach(function (value, index, Array) {
            console.log('index[' + index + ']: ' + value.Score + ', ' + value.CourceId);
        });
    };
    return guestbooklistView;
}());
var guestbooklistController = /** @class */ (function () {
    function guestbooklistController() {
        this.Model = new guestbooklistModel;
        this.View = new guestbooklistView(this.Model);
        this.SubscribeEvents();
        this.Main();
    }
    guestbooklistController.prototype.SubscribeEvents = function () {
    };
    guestbooklistController.prototype.Main = function () {
        var _this = this;
        $.get('/api/Guestbooks/', function (e) { return _this.GetGuestbooks(e); });
    };
    guestbooklistController.prototype.GetGuestbooks = function (e) {
        this.Model.Guestbooklist = e;
        this.View.Update();
    };
    return guestbooklistController;
}());
$().ready(function () {
    var app = new guestbooklistController();
});
//# sourceMappingURL=guestbooklist.js.map