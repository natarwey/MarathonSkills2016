using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarathonSkills2016.Database
{
    public partial class Charity
    {
        public string LogoPath
        {
            get
            {
                var logoFile = !string.IsNullOrEmpty(CharityLogo) ? CharityLogo : "default-logo.png";
                return $"/materials/{logoFile}"; // Обратите внимание на формат пути
            }
        }

        public string ShortDescription
        {
            get
            {
                if (string.IsNullOrEmpty(CharityDescription))
                    return string.Empty;

                return CharityDescription.Length > 50 ?
                    CharityDescription.Substring(0, 50) + "..." :
                    CharityDescription;
            }
        }
        public string CurrentLogoPath
        {
            get
            {
                if (string.IsNullOrEmpty(CharityLogo))
                    return null;

                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "materials", CharityLogo);
                return File.Exists(path) ? path : null;
            }
        }
    }
}
