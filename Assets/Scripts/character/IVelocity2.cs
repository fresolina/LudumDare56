namespace character {
    public interface IVelocity2 : IVelocityX, IVelocityY { }

    public interface IVelocityX {
        float VelocityX { get; set; }
    }

    public interface IVelocityY {
        float VelocityY { get; set; }
    }
}
