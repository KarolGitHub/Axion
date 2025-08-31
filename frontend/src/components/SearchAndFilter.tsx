import React, { useState } from 'react';
import {
  MagnifyingGlassIcon,
  FunnelIcon,
  XMarkIcon,
} from '@heroicons/react/24/outline';

interface FilterOption {
  label: string;
  value: string;
}

interface SearchAndFilterProps {
  onSearch: (query: string) => void;
  onFilter: (filters: Record<string, string>) => void;
  searchPlaceholder?: string;
  filterOptions?: {
    [key: string]: FilterOption[];
  };
  className?: string;
}

export const SearchAndFilter: React.FC<SearchAndFilterProps> = ({
  onSearch,
  onFilter,
  searchPlaceholder = 'Search...',
  filterOptions = {},
  className = '',
}) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [showFilters, setShowFilters] = useState(false);
  const [activeFilters, setActiveFilters] = useState<Record<string, string>>(
    {}
  );

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const query = e.target.value;
    setSearchQuery(query);
    onSearch(query);
  };

  const handleFilterChange = (filterKey: string, value: string) => {
    const newFilters = { ...activeFilters };
    if (value) {
      newFilters[filterKey] = value;
    } else {
      delete newFilters[filterKey];
    }
    setActiveFilters(newFilters);
    onFilter(newFilters);
  };

  const clearAllFilters = () => {
    setActiveFilters({});
    onFilter({});
  };

  const clearFilter = (filterKey: string) => {
    const newFilters = { ...activeFilters };
    delete newFilters[filterKey];
    setActiveFilters(newFilters);
    onFilter(newFilters);
  };

  const hasActiveFilters = Object.keys(activeFilters).length > 0;

  return (
    <div className={`space-y-4 ${className}`}>
      {/* Search Bar */}
      <div className='relative'>
        <div className='absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none'>
          <MagnifyingGlassIcon className='h-5 w-5 text-gray-400' />
        </div>
        <input
          type='text'
          value={searchQuery}
          onChange={handleSearchChange}
          placeholder={searchPlaceholder}
          className='input-field pl-10 pr-10'
        />
        <div className='absolute inset-y-0 right-0 flex items-center'>
          <button
            onClick={() => setShowFilters(!showFilters)}
            className={`p-2 rounded-md transition-colors ${
              showFilters || hasActiveFilters
                ? 'text-blue-600 bg-blue-50'
                : 'text-gray-400 hover:text-gray-600'
            }`}
          >
            <FunnelIcon className='h-5 w-5' />
          </button>
        </div>
      </div>

      {/* Active Filters */}
      {hasActiveFilters && (
        <div className='flex flex-wrap gap-2'>
          {Object.entries(activeFilters).map(([key, value]) => (
            <span
              key={key}
              className='inline-flex items-center px-3 py-1 rounded-full text-sm bg-blue-100 text-blue-800'
            >
              {filterOptions[key]?.find((opt) => opt.value === value)?.label ||
                value}
              <button
                onClick={() => clearFilter(key)}
                className='ml-2 text-blue-600 hover:text-blue-800'
              >
                <XMarkIcon className='h-4 w-4' />
              </button>
            </span>
          ))}
          <button
            onClick={clearAllFilters}
            className='text-sm text-gray-600 hover:text-gray-800 underline'
          >
            Clear all
          </button>
        </div>
      )}

      {/* Filter Panel */}
      {showFilters && (
        <div className='bg-white border border-gray-200 rounded-lg p-4 shadow-sm'>
          <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4'>
            {Object.entries(filterOptions).map(([filterKey, options]) => (
              <div key={filterKey}>
                <label className='block text-sm font-medium text-gray-700 mb-1 capitalize'>
                  {filterKey.replace(/([A-Z])/g, ' $1').trim()}
                </label>
                <select
                  value={activeFilters[filterKey] || ''}
                  onChange={(e) =>
                    handleFilterChange(filterKey, e.target.value)
                  }
                  className='input-field'
                >
                  <option value=''>All</option>
                  {options.map((option) => (
                    <option key={option.value} value={option.value}>
                      {option.label}
                    </option>
                  ))}
                </select>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
