using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public float requiredLaps = 3;

    RaceProgress[] players = new RaceProgress[12];

    private void Start() {
        RemoveAllPlayers();
    }

    public int AddPlayer(RaceProgress player, int id=-1) {
        if (id == -1) {
            for (int i = 0; i < players.Length; i++) {
                if (players[i] != null) {
                    id = i;
                    break;
                }
            }
            if (id == -1) return -1;
        } else if (players[id] != null) return -1;

        players[id] = player;
        return id;
    }

    public void RemovePlayer(int id) {
        players[id] = null;
    }

    public void RemoveAllPlayers() {
        for (int i = 0; i < players.Length; i++) players[i] = null;
    }

    public int GetPlacement(int player) {
        int numAhead = 0;
        for (int i = 0; i < players.Length; i++) {
            if (i == player) continue;
            if (players[i] != null && players[i].Progress > players[player].Progress) numAhead++;
        }
        return numAhead + 1;
    }

    public bool IsFinished(int player) {
        return players[player].Progress >= requiredLaps;
    }
}
