﻿using System.Security.Cryptography;

namespace Infrastructure.JsonProcessing;

public class IdNodesGenerator
{
    public string GenerateSecureIdNodes(int length = 16)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[length / 2];
            
            rng.GetBytes(randomBytes);
            
            return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        }
    }
    
}