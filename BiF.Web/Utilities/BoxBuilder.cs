using System.Collections.Generic;
using System.Linq;
using BiF.DAL.Models;

namespace BiF.Web.Utilities
{
    public class BoxBuilder
    {

        private double? _minOunces;
        private double? _minRating;
        private double? _minCost;

        private double? _totalOunces;
        private double? _totalCost;
        private double? _averageRating;

        private double? _qualifyingOunces;
        private double? _qualifyingCost;
        private double? _qualifyingAverageRating;

        private List<Item> _items;

        public BoxBuilder(List<Item> items) {
            _items = items;
        }

        private void calculateSummary() { 
            _totalOunces = _items?.Where(x => x.Type == "Beer").Sum(x => x.USOunces);
            _totalCost = _items?.Sum(x => x.Cost);
            _averageRating = _items?.Where(x => x.Type == "Beer").Sum(x => x.USOunces * x.UntappdRating) / _totalOunces;

            _qualifyingOunces = _items?.Where(x => x.Type == "Beer" && x.UntappdRating >= _minRating).Sum(x => x.USOunces);
            _qualifyingCost = _items?.Where(x => x.Type == "Beer").Sum(x => x.Cost);
            _qualifyingAverageRating = calculateQualifyingRating();
        }

        //private 

        public double? RequiredCost => _minCost;
        public double? RequiredOunces => _minOunces;
        public double? RequiredRating => _minRating;

        public bool MeetsCost => _minCost == null || _totalCost >= _minCost;
        public bool MeetsOunces => _minOunces == null || _qualifyingOunces >= _minOunces;
        public bool MeetsRating => _minRating == null || _qualifyingAverageRating >= _minRating;

        public double? TotalOunces => _totalOunces;
        public double? TotalCost => _totalCost;
        public double? AverageRating => _averageRating;

        private double? calculateQualifyingRating() {
            IOrderedEnumerable<Item> qualifyingItems = _items.Where(x => x.Type == "Beer").OrderByDescending(x => x.UntappdRating);
            double ounces = 0;
            double rating = 0;
            foreach (var item in qualifyingItems) {
                if (ounces >= _minOunces)
                    break;
                double itemOunces = item.USOunces ?? 0;

                if (ounces + itemOunces > _minOunces) 
                    itemOunces = _minOunces - ounces ?? 0;

                double itemRating = (item.UntappdRating ?? 0) * itemOunces;

                ounces += itemOunces;
                rating += itemRating;
            }

            return rating / ounces;
        }

        public double? QualifyingOunces => _qualifyingOunces;
        public double? QualifyingAverageRating => _qualifyingAverageRating;
        public double? QualifyingCost => _qualifyingCost;

        public BoxBuilder SetMinimumOunces(double minOunces) {
            _minOunces = minOunces;
            calculateSummary();
            return this;
        }

        public BoxBuilder SetMinimumCost(double minCost) {
            _minCost = minCost;
            calculateSummary();
            return this;
        }

        public BoxBuilder SetMinimumRating(double minRating) {
            _minRating = minRating;
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

        public BoxBuilder RemoveMinimumRating() {
            _minRating = null;
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