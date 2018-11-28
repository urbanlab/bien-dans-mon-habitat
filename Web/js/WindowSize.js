//Cada que el usuario cambie de página desde el menú superior, aparecerá la pantalla de carga ocultando el resto de los elementos.

$(function () {
	
	//document.getElementById("p1").innerHTML = $(window).width();
	
	if($(window).width()>1183 && $(window).width()<1305){
		$('.gameC').css("width", "680px");
		$('.gameC').css("height", "400px");
		
		//$('.col-game').css("width", "680px");
		//$('.col-game').css("height", "400px");
	}
	else if($(window).width()<780)
	{
		$('.gameC').css("width", "400px");
		$('.gameC').css("height", "250px");
	}
	else{
		//$('.gameC').css("width", "760px");
		//$('.gameC').css("height", "475px");
		$('.gameC').css("width", "680px");
		$('.gameC').css("height", "400px");
		//$('.col-game').css("width", "680px");
		//$('.col-game').css("height", "400px");
	}
	
	
	$(window).resize(function() {
		//document.getElementById("p1").innerHTML = $(window).width();
  		if($(window).width()>1183 && $(window).width()<1305){
			$('.gameC').css("width", "660px");
			$('.gameC').css("height", "400px");
			
			//$('.col-game').css("width", "680px");
			//$('.col-game').css("height", "400px");
		}
		else if($(window).width()<780)
		{
			$('.gameC').css("width", "400px");
			$('.gameC').css("height", "250px");
		}
		else{
			//$('.gameC').css("width", "760px");
			//$('.gameC').css("height", "475px");
			$('.gameC').css("width", "680px");
			$('.gameC').css("height", "400px");
			//$('.col-game').css("width", "680px");
			//$('.col-game').css("height", "400px");
		}
	});
	
   
});
