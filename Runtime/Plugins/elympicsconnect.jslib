var elympicsconnect = {
	$requestHandler: {
		handleSignTypedData: function (eventName, params) {
			window.dispatchReactUnityEvent(eventName, params);
		},
	},

	DispatchMessage: function (eventName, json) {
		var event = UTF8ToString(eventName);
		var messageStructure = UTF8ToString(json);
		console.log("Got a message from Unity", JSON.parse(messageStructure));

		requestHandler.handleSignTypedData(event, messageStructure);
	},
};

autoAddDeps(elympicsconnect, "$requestHandler");
mergeInto(LibraryManager.library, elympicsconnect);
