using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DataImport.CommonPages;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport
{
    public class ColumnDataImporter
    {
        private readonly IColumnDataImportSettings _settings;

        public ColumnDataImporter(IColumnDataImportSettings settings)
        {
            _settings = settings;
        }

        public void Import()
        {
            var seriesRepo = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
            var sitesRepo = RepositoryFactory.Instance.Get<ISitesRepository>();
            var variablesRepo = RepositoryFactory.Instance.Get<IVariablesRepository>();
            var repoManager = RepositoryFactory.Instance.Get<IRepositoryManager>();

            var toImport = new List<Tuple<ColumnData, Series, OverwriteOptions>> ();
            foreach (var cData in _settings.ColumnDatas.Where(c => c.ImportColumn && c.ColumnName != _settings.DateTimeColumn))
            {
                var site = cData.Site;
                var variable = cData.Variable;

                // Save site if need
                if (!sitesRepo.Exists(site))
                {
                    sitesRepo.AddSite(site);
                }

                // Save Variable if need
                if (!variablesRepo.Exists(variable))
                {
                    variablesRepo.Insert(variable);
                }

                //
                OverwriteOptions options;
                if (seriesRepo.ExistsSeries(site, variable))
                {
                    using (var dialog = new ExistsSeriesQuestionDialog(site.Name, variable.Name))
                    {
                        dialog.ShowDialog();
                        options = dialog.CurrentOption;
                    }
                }
                else
                {
                    options = OverwriteOptions.Overwrite;
                }

                var series = new Series(site, variable, null, null, null);
                toImport.Add(new Tuple<ColumnData, Series, OverwriteOptions>(cData, series, options));
            }


            foreach (DataRow row in _settings.Data.Rows)
            {
                DateTime dateTime;
                if (!DateTime.TryParse(row[_settings.DateTimeColumn].ToString(), out dateTime))
                    continue;

                foreach (var tuple in toImport)
                {
                    var columnIndex = tuple.Item1.ColumnIndex;

                    Double value;
                    if (!Double.TryParse(row[columnIndex].ToString(), out value))
                        continue;

                    var series = tuple.Item2;
                    series.DataValueList.Add(new DataValue(value, dateTime, 0));
                }
            }

            var theme = new Theme(Path.GetFileNameWithoutExtension(_settings.PathToFile));
            foreach (var tuple in toImport)
            {
                repoManager.SaveSeries(tuple.Item2, theme, tuple.Item3);
            }
        }
    }
}