using FishNet.Object;

public class Server : NetworkBehaviour {
    public static Server instance;

    private void Awake() {
        instance = this;
    }
}
