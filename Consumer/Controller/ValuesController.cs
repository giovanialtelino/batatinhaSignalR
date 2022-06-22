using BatatinhaSignalR.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BatatinhaSignalR.Controller
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class ValuesController : ControllerBase
    //{
    //    private readonly ChannelWriter<ValueControllerModelOne> _channelWriterOne;
    //    private readonly ChannelWriter<ValueControllerModelTwo> _channelWriterTwo;  

    //    public ValuesController(ChannelWriter<ValueControllerModelOne> channelWriterOne, ChannelWriter<ValueControllerModelTwo> channelWriterTwo)
    //    {
    //        _channelWriterOne = channelWriterOne;
    //        _channelWriterTwo = channelWriterTwo;
    //    }

    //    [HttpGet("one")]
    //    public async Task ReceiveOne([FromQuery] string valor)
    //    {
    //        Console.WriteLine(valor);

    //        var decimalValor = new ValueControllerModelOne { Value = decimal.Parse(valor) };

    //        _channelWriterOne.TryWrite(decimalValor);
    //    }

    //    [HttpGet("two")]
    //    public async Task ReceiveTwo([FromQuery] string valor)
    //    {
    //        Console.WriteLine(valor);

    //        var decimalValor = new ValueControllerModelTwo { Value = decimal.Parse(valor) };

    //        _channelWriterTwo.TryWrite(decimalValor);
    //    }
    //}
}
