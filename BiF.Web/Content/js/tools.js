
var Popup = function() {
	this._popup = $('.popup');
	this.Show = function(msg) {
		$('.overlay').show().css('display', 'flex');

		if (msg !== '')
			this._popup.html(msg);
		this._popup.show();
	};

	this.Hide = function() {
		$('.overlay').hide();
		this._popup.hide();
	};

};

var popup;

$(function() {
	popup = new Popup();
});