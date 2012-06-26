function addOnBlur(obj)
{
	obj.attachEvent('onfocus',focusInput);

	obj.attachEvent('onblur',blurInput);
}

function focusInput(d)
{
	var b=true;
	var a=(window.event?window.event:d);
	var c=(window.event?a.srcElement:a.target);
	/*if(!c.objParams){
		return true
	}*/
	c.value = c.value.substring(1, c.value.length);
	return b;
}

function blurInput(d)
{
	var b=true;
	var a=(window.event?window.event:d);
	var c=(window.event?a.srcElement:a.target);
	/*if(!c.objParams){
		return true
	}*/
	c.value = "$"+c.value;
	return b;
}