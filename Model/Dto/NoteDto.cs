using System.Collections.Generic;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model.Dto
{
    /// <summary>
    /// Dto object
    /// </summary>
    public class NoteDto
    {
        public string Id { get; set; }
        public User Author { get; set; }
        public IEnumerable<string> Tag { get; set; }
        public string Text { get; set; }
    }
}