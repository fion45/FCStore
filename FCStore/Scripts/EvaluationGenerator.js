function GenerateEvaluation(BDT, AddSecondCount) {
    var tmpAS = Math.random() * AddSecondCount;
    var tmpDT = AddSeconds(BDT,tmpAS);
    var result = {};
    var index = Math.min(Math.floor(Math.random() * NameArr.length),NameArr.length - 1);
    var nameStr = NameArr[index];
    var tmpName = "";
    if(nameStr.length >= 3) {
    	var tmpL = nameStr.length / 3;
    	tmpName = nameStr.substring(0,tmpL);
    	for(var i=0;i<nameStr.length - 2 * tmpL; i++) {
    		tmpName += "*";
    	}
    	tmpName += nameStr.substring(nameStr.length - tmpL);
    }
    else {
    	tmpName = nameStr.substring(0,1) + *;
    }
    result.IDLabel = tmpName;
    result.DataTime = tmpDT;
    index = Math.min(Math.floor(Math.random() * ContentArr.length),ContentArr.length - 1);
    result.Description = ContentArr[index];
    return result;
};

var ContentArr = [
    "送货快，服务好",
    "产品性价比高",
    "物优价廉"
];

var NameArr = [
    "苏尔康",
    "苏玲连",
    "霍田"
];