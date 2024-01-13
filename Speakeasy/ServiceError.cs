namespace Speakeasy;

public enum ApiError
{
	MessageHandlerFailure = 5000,
	UnknownContract,
	MessageProcessingFailure,
	ApiProxyException,
	AccountUpdateFailed,
	AccountNotReady,
	ObjectReadError,
	MissingUpdateAsset,
	UnhandledException,
	ThreadException,
	AccountReadError,
	ProcessAccessDenied,
	MethodCalledBeforeInitialization,
	StaticDataReadError,
}
