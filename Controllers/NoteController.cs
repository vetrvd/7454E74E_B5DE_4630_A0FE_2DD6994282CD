using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model;
using _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model.Dto;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NoteController : ControllerBase
    {
        private readonly ILogger<NoteController> _logger;
        private readonly IRepository<Note> _repository;
        private readonly IMapper _mapper;

        public NoteController(
            ILogger<NoteController> logger,
            IRepository<Note> repository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDto>> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var result = await _repository.GetById(id);
            if (result == null)
            {
                return NotFound();
            }

            return _mapper.Map<NoteDto>(result);
        }
        
        [HttpGet]
        public async Task<IEnumerable<NoteDto>> Get([FromQuery] FilterDto filter)
        {
            //Create custom builder(save 1 abstraction) 
            var builder = new List<string>();
            if (!String.IsNullOrEmpty(filter.User))
            {
                builder.Add($@"""{nameof(Note.Author)}.{nameof(Model.User.Name)}"": ""{filter.User}""");
            }

            if (!String.IsNullOrEmpty(filter.Text))
            {
                builder.Add($@"""{nameof(Note.Text)}"": {{$regex: "".*{filter.Text}.*""}}");
            }
            
            if (filter.Tag != null)
            {
                builder.Add($@"""{nameof(Note.Tag)}"": ""{filter.Tag}"" ");
            }

            return (await _repository.Get($@"{{ {String.Join(",",builder)} }}", null, null)).Select(_mapper.Map<NoteDto>);
        }
        

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NoteDto note)
        {
            if (note == null)
            {
                return BadRequest();
            }
            
            var item = _mapper.Map<Note>(note);
            var value = await _repository.Add(item);
            return CreatedAtAction(nameof(Get), new { id = value.Id}, value.Id);

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            
            _repository.Delete(id);
            return Ok();
        }
    }
}
