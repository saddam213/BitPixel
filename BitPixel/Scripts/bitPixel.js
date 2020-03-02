const store = new Storage();
function Storage() {
	this.get = function (name) {
		return JSON.parse(window.localStorage.getItem(name));
	};

	this.set = function (name, value) {
		window.localStorage.setItem(name, JSON.stringify(value));
	};

	this.remove = function (name) {
		window.localStorage.removeItem(name);
	};

	this.clear = function () {
		window.localStorage.clear();
	};
}

const datatable_Layout = "<'d-flex flex-column'<'w-100'rt> <'w-100'<'d-flex justify-content-between'<''i><''p>>>";
const requestVerificationToken = $('input[name="__RequestVerificationToken"]').val();
$.ajaxPrefilter(function (options, originalOptions) {
	options.async = true;
	if (options.type.toUpperCase() == "POST") {
		options.data = $.param($.extend(originalOptions.data, { __RequestVerificationToken: requestVerificationToken }));
	}
});
$.ajaxSetup({ cache: false });


const postJson = (url, vars) => {
	return $.ajax({
		url: url,
		cache: false,
		async: true,
		type: "POST",
		dataType: 'json',
		data: vars
	});
}

const getJson = (url, vars) => {
	return $.ajax({
		url: url,
		cache: false,
		async: true,
		type: "GET",
		dataType: 'json',
		data: vars
	});
}

const getMousePos = (canvas, evt) => {
	const rect = canvas.getBoundingClientRect();
	return {
		x: evt.clientX - rect.left,
		y: evt.clientY - rect.top
	};
}


const throttleFunction = (func, limit) => {
	let lastFunc
	let lastRan
	return function () {
		const context = this
		const args = arguments
		if (!lastRan) {
			func.apply(context, args)
			lastRan = Date.now()
		} else {
			clearTimeout(lastFunc)
			lastFunc = setTimeout(function () {
				if ((Date.now() - lastRan) >= limit) {
					func.apply(context, args)
					lastRan = Date.now()
				}
			}, limit - (Date.now() - lastRan))
		}
	}
}


function fallbackCopyTextToClipboard(text) {
	var textArea = document.createElement("textarea");
	textArea.value = text;
	textArea.style.position = "fixed";  //avoid scrolling to bottom
	document.body.appendChild(textArea);
	textArea.focus();
	textArea.select();

	try {
		var successful = document.execCommand('copy');
		var msg = successful ? 'successful' : 'unsuccessful';
		console.log('Fallback: Copying text command was ' + msg);
	} catch (err) {
		console.error('Fallback: Oops, unable to copy', err);
	}

	document.body.removeChild(textArea);
}
function copyTextToClipboard(text) {
	if (!navigator.clipboard) {
		fallbackCopyTextToClipboard(text);
		return;
	}
	navigator.clipboard.writeText(text).then(function () {
		console.log('Async: Copying to clipboard was successful!');
	}, function (err) {
		console.error('Async: Could not copy text: ', err);
	});
}



const toLocalTime = (time) => {
	if (!time || time.length == 0) {
		return '';
	}
	return moment.utc(time).local().format("D/MM/YYYY h:mm:ss A")
}

const toLocalTimeShort = (time) => {
	if (!time || time.length == 0) {
		return '';
	}
	return moment.utc(time).local().format("D/MM h:mm A")
}

const toLocalDate = (time) => {
	if (!time || time.length == 0) {
		return '';
	}
	return moment.utc(time).local().format("DD/MM/YYYY")
}

const toLocalDateShort = (time) => {
	if (!time || time.length == 0) {
		return '';
	}
	return moment.utc(time).local().format("DD/MM")
}

const toUtcTime = (time) => {
	if (!time || time.length == 0) {
		return '';
	}
	return moment.utc(time).format("D/MM/YYYY h:mm:ss A")
}

const toUtcTimeShort = (time) => {
	if (!time || time.length == 0) {
		return '';
	}
	return moment.utc(time).format("D/MM h:mm A")
}











// numeric title search 'title="number"'
jQuery.extend(jQuery.fn.dataTable.ext.oSort, {
	"title-numeric-pre": function (a) {
		var x = a.match(/title="*(-?[0-9\.]+)/)[1];
		return parseFloat(x);
	},

	"title-numeric-asc": function (a, b) {
		return a < b ? -1 : a > b ? 1 : 0;
	},

	"title-numeric-desc": function (a, b) {
		return a < b ? 1 : a > b ? -1 : 0;
	}
});



jQuery.extend(true, jQuery.fn.dataTable.ext.defaults, {
	//dom:"<'row'<'col-sm-12'tr>><'datatable-length-row'l> <'datatable-export-row'B><'datatable-page-row'p><'clearfix'><'datatable-info-row'i>",
	dom: "<'d-flex flex-column'<'w-100'rt> <'w-100'<'d-flex justify-content-between'<''i><''p>>>",
	renderer: 'bootstrap'
});

/* Default class modification */
jQuery.extend(jQuery.fn.dataTable.ext.classes, {
	sPageButton: "paginate_button btn btn-outline-secondary ml-1"
});

jQuery.extend(jQuery.fn.dataTable.ext, {
	errMode: "none"
});




const loadBackgroundImage = (imageName) => {
	return new Promise(function (resolve, reject) {
		const image = new Image();
		const cacheBreak = Math.round((new Date()).getTime());
		image.src = imageName + cacheBreak;
		image.onload = function () {
			return resolve(image);
		}
		image.onerror = function () {
			return resolve(undefined);
		}
	});
}



const addCanvasPixel = async (context, x, y, color) => {
	//context.beginPath();
	context.fillStyle = color;
	context.fillRect(x, y, 1, 1);
	//context.closePath();
}

const getCanvasLocation = (canvas, scale, mouseEvent) => {
	const rect = canvas.getBoundingClientRect();
	return {
		x: parseInt((mouseEvent.clientX - rect.left) / scale),
		y: parseInt((mouseEvent.clientY - rect.top) / scale)
	}
}

const getCacheVersion = () => {
	const milliseconds = new Date().getTime();
	return ~~((milliseconds / 1000) / 60);
}


String.prototype.paddingLeft = function (paddingValue) {
	return String(paddingValue + this).slice(-paddingValue.length);
};



const formatDuration = (duration) => {
	let output = '';
	const days = Math.floor(duration.asDays());
	if (days > 0) {
		output = `${days.toString().paddingLeft(days > 99 ? "000" : "00")}:`;
	}
	return `${output}${duration.hours().toString().paddingLeft("00")}:${duration.minutes().toString().paddingLeft("00")}:${duration.seconds().toString().paddingLeft("00")}`;
}