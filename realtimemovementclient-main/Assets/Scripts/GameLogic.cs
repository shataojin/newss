using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    GameObject character;
   
    Vector2 characterPositionInPercent;
    Vector2 characterVelocityInPercent;
    const float CharacterSpeed = 0.25f;
    float DiagonalCharacterSpeed;

    void Start()
    {
        DiagonalCharacterSpeed = Mathf.Sqrt(CharacterSpeed * CharacterSpeed + CharacterSpeed * CharacterSpeed) / 2f;
        NetworkClientProcessing.SetGameLogic(this);

        Sprite circleTexture = Resources.Load<Sprite>("Circle");

        character = new GameObject("Character");

        character.AddComponent<SpriteRenderer>();
        character.GetComponent<SpriteRenderer>().sprite = circleTexture;

        Vector2 initialBalloonPosition = new Vector2(Random.value, Random.value);
        string positionMessage = $"{ClientToServerSignifiers.SendBalloonPosition},{initialBalloonPosition.x},{initialBalloonPosition.y}";
        NetworkClientProcessing.SendMessageToServer(positionMessage, TransportPipeline.ReliableAndInOrder);
    }

    void Update()
    {

        characterPositionInPercent += (characterVelocityInPercent * Time.deltaTime);

        // 发送位置到服务器
        string positionMessage = $"{ClientToServerSignifiers.PlayerPosition},{characterPositionInPercent.x},{characterPositionInPercent.y}";
        NetworkClientProcessing.SendMessageToServer(positionMessage, TransportPipeline.ReliableAndInOrder);

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)
            || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            characterVelocityInPercent = Vector2.zero;

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                characterVelocityInPercent.x = DiagonalCharacterSpeed;
                characterVelocityInPercent.y = DiagonalCharacterSpeed;
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                characterVelocityInPercent.x = -DiagonalCharacterSpeed;
                characterVelocityInPercent.y = DiagonalCharacterSpeed;
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                characterVelocityInPercent.x = DiagonalCharacterSpeed;
                characterVelocityInPercent.y = -DiagonalCharacterSpeed;
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                characterVelocityInPercent.x = -DiagonalCharacterSpeed;
                characterVelocityInPercent.y = -DiagonalCharacterSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
                characterVelocityInPercent.x = CharacterSpeed;
            else if (Input.GetKey(KeyCode.A))
                characterVelocityInPercent.x = -CharacterSpeed;
            else if (Input.GetKey(KeyCode.W))
                characterVelocityInPercent.y = CharacterSpeed;
            else if (Input.GetKey(KeyCode.S))
                characterVelocityInPercent.y = -CharacterSpeed;
        }

        characterPositionInPercent += (characterVelocityInPercent * Time.deltaTime);

        Vector2 screenPos = new Vector2(characterPositionInPercent.x * (float)Screen.width, characterPositionInPercent.y * (float)Screen.height);
        Vector3 characterPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        characterPos.z = 0;
        character.transform.position = characterPos;

    }
    Dictionary<int, GameObject> balloons = new Dictionary<int, GameObject>();

    public void CreateBalloon(Vector2 position, int playerID)
    {
        if (balloons.ContainsKey(playerID))
        {
            // 更新现有气球位置
            balloons[playerID].transform.position = GetWorldPosition(position);
        }
        else
        {
            // 创建新气球
            GameObject balloon = new GameObject($"Balloon_{playerID}");
            Sprite balloonSprite = Resources.Load<Sprite>("Circle");
            balloon.AddComponent<SpriteRenderer>().sprite = balloonSprite;

            balloon.transform.position = GetWorldPosition(position);
            balloons.Add(playerID, balloon);
        }
    }

    Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Vector2 screenPos = new Vector2(screenPosition.x * Screen.width, screenPosition.y * Screen.height);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPosition.z = 0;
        return worldPosition;
    }



}
