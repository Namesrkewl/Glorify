using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Identifiable {
    Key GetKey();
    void SetKey(Key key);
}
