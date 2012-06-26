function Dictionary() 
{
    this.dict = [];
}

Dictionary.prototype.Add = function(key, value) 
{
    this.dict[key] = value;
}

Dictionary.prototype.Remove = function(key) 
{
    this.dict[key] = null;
}

Dictionary.prototype.GetValue = function(key) 
{
    return this.dict[key];
}

Dictionary.prototype.Clear = function() 
{
    this.dict = [];
}