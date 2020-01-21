public class EnemyFireBackwards : Enemy
{
    protected override void Fire(bool isDirectionDown)
    {
        var playerY = this.Player.transform.position.y;
        //// print($"BackFire: {playerY > this.transform.position.y} {playerY} > {this.transform.position.y}");
        if (playerY > this.transform.position.y)
        {
            isDirectionDown = false;
        }

        if (SpriteRenderer.flipY = !isDirectionDown)
        {
            SpriteRenderer.flipY = !isDirectionDown;
        }

        
        base.Fire(isDirectionDown);
    }
}
