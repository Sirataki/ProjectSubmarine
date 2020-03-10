using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCount : MonoBehaviour {

    public int shipPoints = 0;

    public int achieveCount = 0;

    [SerializeField]
    private ParticleSystem trashBubble;

    [SerializeField]
    private GameObject inScorePointText;

    [SerializeField]
    private List<Material> bubbleMtList;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Trash")
        {
            int p = other.gameObject.GetComponent<Trashes>().pointsSh;
            shipPoints += p;
            achieveCount++;
            //SE
            soundManager.Instance.PlaySound(4, false);

            //スコアのUI表示
            GameObject scoreText = Instantiate(inScorePointText, other.gameObject.transform.position, Quaternion.identity) as GameObject;
            scoreText.transform.position = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z - 2);
            scoreText.GetComponent<InScoreText>().PointSet(p);

            ParticlePlay(p, other.transform.position);
            Destroy(other.gameObject);
        }
    }

    //パーティクルの設定
    private void ParticlePlay(int p, Vector3 pos)
    {
        var pMain = trashBubble.main;
        pMain.startSize = (float)p / 5.0f;
        trashBubble.transform.position = pos;
        trashBubble.Play();
    }

}
