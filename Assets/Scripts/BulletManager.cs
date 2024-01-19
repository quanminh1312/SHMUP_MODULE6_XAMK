using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class BulletManager : MonoBehaviour
{
    public Bullet[] bulletPrefabs;
    public enum BulletType
    {
        bullet1_Size1,
        Bullet1_Size2,
        Bullet1_Size3,
        Bullet1_Size4,
        Bullet1_Size5,
        Bullet2_Size1,
        Bullet2_Size2,
        Bullet2_Size3,
        Bullet2_Size4,
        Bullet2_Size5,
        Bullet3_Size1,
        Bullet3_Size2,
        Bullet3_Size3,
        Bullet3_Size4,
        Bullet3_Size5,
        Bullet4_Size1,
        Bullet4_Size2,
        Bullet4_Size3,
        Bullet4_Size4,
        Bullet4_Size5,
        Bullet5_Size1,
        Bullet5_Size2,
        Bullet5_Size3,
        Bullet5_Size4,
        Bullet5_Size5,
        Bullet6_Size1,
        Bullet6_Size2,
        Bullet6_Size3,
        Bullet6_Size4,
        Bullet6_Size5,
        MAX_TYPES
    }

    const int MAX_BULLET_PER_TYPE = 500;
    const int MAX_BULLET_COUNT = (int)BulletType.MAX_TYPES * MAX_BULLET_PER_TYPE;
    private Bullet[] bullets = new Bullet[MAX_BULLET_COUNT];
    private NativeArray<BulletData> bulletData;
    private TransformAccessArray bulletTransforms;
    ProcessBulletJob jobProcessor;
    void Start()
    {
        bulletData = new NativeArray<BulletData>(MAX_BULLET_COUNT, Allocator.Persistent);
        bulletTransforms = new TransformAccessArray(MAX_BULLET_COUNT);

        int index = 0;
        for (int bulletType = (int)BulletType.bullet1_Size1; bulletType< (int) BulletType.MAX_TYPES; bulletType++)
        {
            for (int bulletIndex = 0; bulletIndex < MAX_BULLET_PER_TYPE; bulletIndex++)
            {
                Bullet newBullet = Instantiate(bulletPrefabs[bulletType]).GetComponent<Bullet>();
                newBullet.index = index;
                newBullet.gameObject.SetActive(false);
                newBullet.transform.SetParent(transform);
                bullets[index] = newBullet;
                bulletTransforms.Add(newBullet.transform);
                index++;
            }
        }

        jobProcessor = new ProcessBulletJob {bullets = bulletData };
    }
    private void OnDestroy()
    {
        bulletData.Dispose();
        bulletTransforms.Dispose();
    }
    private int NextFreeBulletIndex(BulletType bulletType)
    {
        int index = (int)bulletType * MAX_BULLET_PER_TYPE;
        for (int i = 0; i < MAX_BULLET_PER_TYPE; i++)
        {
            if (!bulletData[index + i].active) return (index + i);
        }
        return -1;
    }
    public Bullet SpawnBullet(BulletType bulletType, float x, float y, float dx, float dy, float angle, float dAngle, bool homing, byte playerIndex)
    {
        int index = NextFreeBulletIndex(bulletType);

        if (index == -1) return null;

        Bullet result = bullets[index];
        result.playerIndex = playerIndex;
        result.gameObject.SetActive(true);
        bulletData[index] = new BulletData(x, y, dx, dy, angle,dAngle, (int)bulletType, true,homing);
        bullets[index].gameObject.transform.position = new Vector3(x, y, 0);
        return result;
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance && GameManager.Instance.playerCrafts[0])//error
            jobProcessor.playerPosition = GameManager.Instance.playerCrafts[0].transform.position;//error
        else
            jobProcessor.playerPosition = new Vector2(-999,-999);



        if (GameManager.Instance && GameManager.Instance.progressWindow)
            jobProcessor.progressY = GameManager.Instance.progressWindow.transform.position.y;
        else
            jobProcessor.progressY = 0;


        ProcessBullet();

        for (int i = 0; i < MAX_BULLET_COUNT; i++)
        {
            if (!bulletData[i].active) 
                bullets[i].gameObject.SetActive(false);
        }
    }
    public void DeActivateBullet(int index)
    {
        bullets[index].gameObject.SetActive(false);
        float x = bulletData[index].positionX;
        float y = bulletData[index].positionY;
        float dx = bulletData[index].dX;
        float dy = bulletData[index].dY;
        float angle = bulletData[index].angle;
        int type = bulletData[index].type;
        float dAngle = bulletData[index].dAngle;
        bool homing = bulletData[index].homing;
        bulletData[index] = new BulletData(x, y, dx, dy, angle,dAngle, type, false,homing);
    }
    public void ProcessBullet()
    {
        JobHandle handle =  jobProcessor.Schedule(bulletTransforms);
        handle.Complete();
    }
    public struct ProcessBulletJob: IJobParallelForTransform
    {
        public NativeArray<BulletData> bullets;
        public Vector2 playerPosition;
        public float progressY;
        public void Execute(int index, TransformAccess transform)
        {
            bool active = bullets[index].active;
            if (!active) return;
            float dx = bullets[index].dX;
            float dy = bullets[index].dY;
            float angle = bullets[index].angle;
            float x = bullets[index].positionX;
            float y = bullets[index].positionY;
            int type = bullets[index].type;
            float dAngle = bullets[index].dAngle;
            bool homing = bullets[index].homing;

            //homing    
            if (homing && playerPosition.x < -998)
            {
                active = false;
            }
            else if (homing)
            {
                Vector2 velocity = new Vector2(dx, dy);
                float speed = velocity.magnitude;
                Vector2 toPlayer = new Vector2(playerPosition.x - x, playerPosition.y - y);
                Vector2 newVelocity = Vector2.Lerp(velocity.normalized,toPlayer.normalized, 0.05f);
                newVelocity.Normalize();
                newVelocity *= speed;
                dx = newVelocity.x;
                dy = newVelocity.y;
            }
            x += dx;
            y += dy;

            //check for out of bounds
            if (x < -320 || x >320 || y-progressY <-180 || y-progressY >180)
            {
                active = false;
            }
            bullets[index] = new BulletData(x, y, dx, dy, angle,dAngle, type, active,homing);
            if (active) 
            {
                Vector3 newPosition = new Vector3(x, y, 0);
                transform.position = newPosition;
                transform.rotation = Quaternion.LookRotation(Vector3.forward,new Vector3(dx,dy,0));
            }
        }
    }
}
