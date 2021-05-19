using System.Collections.Generic;
using System.Linq;
using BiF.DAL.Models;

namespace BiF.Web.Utilities
{
    public class BoxBuilder
    {

        private double? _minOunces;
        private double? _minBeerRating;
        private double? _minBoxRating;
        private double? _minCost;
        private int? _minUnique;

        private double? _totalOunces;
        private double? _totalCost;
        private double? _minimumBeerRating;
        private double? _averageBoxRating;
        private int? _totalUnique;

        private double? _qualifyingOunces;
        private double? _qualifyingCost;
        private double? _qualifyingBeerRating;
        private double? _qualifyingBoxRating;

        private List<Item> _items;

        public BoxBuilder(List<Item> items) {
            _items = items;
        }

        private void calculateSummary() { 
            _totalOunces = _items?.Where(x => x.Type == "Beer").Sum(x => x.USOunces);
            _totalCost = _items?.Sum(x => x.Cost);
            _averageBoxRating = _totalOunces == 0 ? 0 : _items?.Where(x => x.Type == "Beer").Sum(x => x.USOunces * x.UntappdRating) / _totalOunces;
            _minimumBeerRating = _items?.Where(x => x.UntappdRating > 0).Min(x => x.UntappdRating);
            _totalUnique = _items?.Where(x => x.Type == "Beer").Select(x => x.UntappdId).Distinct().Count();

            _qualifyingOunces = _items?.Where(x => x.Type == "Beer" && x.UntappdRating >= (_minBeerRating ?? -1)).Sum(x => x.USOunces);
            _qualifyingCost = _items?.Where(x => x.Type == "Beer").Sum(x => x.Cost);
            _qualifyingBoxRating = calculateQualifyingBoxRating();
            _qualifyingBeerRating = calculateQualifyingBeerRating();
        }

        //private 

        public double? RequiredCost => _minCost;
        public double? RequiredOunces => _minOunces;
        public double? RequiredBeerRating => _minBeerRating;
        public double? RequiredBoxRating => _minBoxRating;
        public double? RequiredUnique => _minUnique;

        public bool MeetsCost => _minCost == null || _totalCost >= _minCost;
        public bool MeetsOunces => _minOunces == null || _qualifyingOunces >= _minOunces;
        public bool MeetsBeerRating => _minBeerRating == null || _qualifyingBeerRating >= _minBeerRating;
        public bool MeetsBoxRating => _minBoxRating == null || _qualifyingBoxRating >= _minBoxRating;
        public bool MeetsUnique => _minUnique == null || _totalUnique >= _minUnique;

        public double? TotalOunces => _totalOunces;
        public double? TotalCost => _totalCost;
        public double? AverageBoxRating => _averageBoxRating;
        public double? MinimumBeerRating => _minimumBeerRating;
        public double? TotalUnique => _totalUnique;

        private double? calculateQualifyingBeerRating() {
            IOrderedEnumerable<Item> qualifyingItems = _items.Where(x => x.Type == "Beer").OrderByDescending(x => x.UntappdRating);
            double ounces = 0;
            double rating = 0;
            foreach (Item item in qualifyingItems) {
                if (ounces >= _minOunces)
                    break;
                double itemOunces = item.USOunces ?? 0;

                if (ounces + itemOunces > _minOunces)
                    itemOunces = _minOunces - ounces ?? 0;

                rating = item.UntappdRating ?? 0;

                ounces += itemOunces;
            }

            return rating;
        }

        private double? calculateQualifyingBoxRating() {
            IOrderedEnumerable<Item> qualifyingItems = _items.Where(x => x.Type == "Beer").OrderByDescending(x => x.UntappdRating);
            double ounces = 0;
            double rating = 0;
            foreach (Item item in qualifyingItems) {
                if (ounces >= _minOunces)
                    break;
                double itemOunces = item.USOunces ?? 0;

                if (ounces + itemOunces > _minOunces) 
                    itemOunces = _minOunces - ounces ?? 0;

                double itemRating = (item.UntappdRating ?? 0) * itemOunces;

                ounces += itemOunces;
                rating += itemRating;
            }

            return ounces == 0 ? 0 : rating / ounces;
        }

        public double? QualifyingOunces => _qualifyingOunces;
        public double? QualifyingBeerRating => _qualifyingBeerRating;
        public double? QualifyingBoxRating => _qualifyingBoxRating;
        public double? QualifyingCost => _qualifyingCost;

        public BoxBuilder SetMinimumOunces(double? minOunces) {
            _minOunces = minOunces;
            calculateSummary();
            return this;
        }

        public BoxBuilder SetMinimumCost(double? minCost) {
            _minCost = minCost;
            calculateSummary();
            return this;
        }

        public BoxBuilder SetMinimumBeerRating(double? minRating) {
            _minBeerRating = minRating;
            calculateSummary();
            return this;
        }

        public BoxBuilder SetMinimumBoxRating(double? minRating)
        {
            _minBoxRating = minRating;
            calculateSummary();
            return this;
        }

        public BoxBuilder SetMinimumUnique(int? minUnique)
        {
            _minUnique = minUnique;
            calculateSummary();
            return this;
        }

        public BoxBuilder RemoveMinimumOunces() {
            _minOunces = null;
            return this;
        }

        public BoxBuilder RemoveMinimumCost() {
            _minCost = null;
            calculateSummary();
            return this;
        }

        public BoxBuilder RemoveMinimumBeerRating() {
            _minBeerRating = null;
            calculateSummary();
            return this;
        }

        public BoxBuilder RemoveMinimumBoxRating()
        {
            _minBoxRating = null;
            calculateSummary();
            return this;
        }

        public BoxBuilder RemoveUnique()
        {
            _minUnique = null;
            calculateSummary();
            return this;
        }

        //private bool? validateOunces => _items.Where(x => x.Type == "Beer").Sum(x => x.USOunces) > _minOunces;
        //private bool? validateCost => Items.Sum(x => x.Cost) > _minCost;
        //private bool? validateRating => Items.Where(x => x.UntappdId != null).Sum(x => x.UntappdRating) > _minRating;

        //public object Summarize() {
        //    double? totalOunces = Items.Where(x => x.Type == "Beer").Sum(x => x.USOunces);
        //    double? totalCost = Items.Sum(x => x.Cost);
        //    double? totalRating = Items.Where(x => x.UntappdId != null).Sum(x => x.UntappdRating * x.USOunces) / totalOunces;

        //    return null;
        //}

    }
}