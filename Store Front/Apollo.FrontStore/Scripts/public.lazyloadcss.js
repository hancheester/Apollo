(function () {
	var head = document.getElementsByTagName('head')[0];
	var element1 = document.createElement('link');
	element1.rel = 'stylesheet';
	element1.type = 'text/css';
	element1.href = 'https://fonts.googleapis.com/css?family=Archivo+Narrow:400,400italic,700,700italic';
	element1.media = 'non-existant-media';
	head.insertBefore(element1, head.firstChild);
	
	var element2 = document.createElement('link');
	element2.rel = 'stylesheet';
	element2.type = 'text/css';
	element2.href = 'https://fonts.googleapis.com/css?family=Roboto+Slab:400,300,100,700';
	element2.media = 'non-existant-media';
	head.insertBefore(element2, head.firstChild);
	
	var element3 = document.createElement('link');
	element3.rel = 'stylesheet';
	element3.type = 'text/css';
	element3.href = 'https://fonts.googleapis.com/css?family=Roboto:400,100,100italic,300,300italic,400italic,500,500italic,700,700italic,900,900italic';
	element3.media = 'non-existant-media';
	head.insertBefore(element3, head.firstChild);
	
	var element4 = document.createElement('link');
	element4.rel = 'stylesheet';
	element4.type = 'text/css';
	element4.href = 'https://fonts.googleapis.com/css?family=Montserrat:400,700';
	element4.media = 'non-existant-media';
	head.insertBefore(element4, head.firstChild);
	
	var element5 = document.createElement('link');
	element5.rel = 'stylesheet';
	element5.type = 'text/css';
	element5.href = 'https://fonts.googleapis.com/css?family=Droid+Serif:400,400italic,700,700italic';
	element5.media = 'non-existant-media';
	head.insertBefore(element5, head.firstChild);
	
	var element6 = document.createElement('link');
	element6.rel = 'stylesheet';
	element6.type = 'text/css';
	element6.href = 'https://fonts.googleapis.com/css?family=Lato:400,100,100italic,300,300italic,400italic,700italic,900,900italic,700';
	element6.media = 'non-existant-media';
	head.insertBefore(element6, head.firstChild);
		
	var element7 = document.createElement('link');
	element7.rel = 'stylesheet';
	element7.type = 'text/css';
	element7.href = 'https://cdnjs.cloudflare.com/ajax/libs/flag-icon-css/2.1.0/css/flag-icon.min.css';
	element7.media = 'non-existant-media';
	head.insertBefore(element7, head.firstChild);

	setTimeout(function () {
		element1.media = 'all';
		element2.media = 'all';
		element3.media = 'all';
		element4.media = 'all';
		element5.media = 'all';
		element6.media = 'all';
		element7.media = 'all';
	});

	window.onload = function ()
	{ document.getElementById("hideAll").style.visibility = "visible"; }
})();