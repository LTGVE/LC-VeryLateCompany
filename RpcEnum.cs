using Unity.Netcode;

internal class RpcEnum : NetworkBehaviour
{
	public static int None => 0;

	public static int Client => 2;

	public static int Server => 1;
	public static int Execute => 1;

}
