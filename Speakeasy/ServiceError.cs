namespace Speakeasy;

public enum ApiError
{
	MessageHandlerFailure = 5000,
	UnknownContract,
	MessageProcessingFailure,
	ApiProxyException,
	ObjectReadError,
	MissingUpdateAsset,
	UnhandledException,
	ThreadException,
	ProcessAccessDenied,
	MethodCalledBeforeInitialization,
	StaticDataReadError,
}
