import 'package:flutter/material.dart';
import 'package:grooveon_desktop/models/response/genre_stat_item_response.dart';
import 'package:grooveon_desktop/models/response/music_stat_item_response.dart';
import 'package:provider/provider.dart';
import '../models/response/music_overview_response.dart';
import '../providers/report_provider.dart';
import '../utils/session.dart';

class MusicOverContent extends StatefulWidget {
  const MusicOverContent({super.key});

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color cardColor = Colors.white;
  static const Color borderColor = Color(0xFFD9D9DE);
  static const Color textColor = Color(0xFF222222);
  static const Color subTextColor = Color(0xFF6F6F78);
  static const Color softBackground = Color(0xFFF7F7F9);

  @override
  State<MusicOverContent> createState() => _MusicOverContentState();
}

class _MusicOverContentState extends State<MusicOverContent> {
  bool _isYearMode = true;
  int _selectedYear = 2025;
  int _selectedMonth = 1;

  bool _isLoading = false;
  String? _error;
  MusicOverviewResponse? _data;

  static const List<int> _years = [2025, 2026];

  static const List<_MonthItem> _months = [
    _MonthItem(1, "January"),
    _MonthItem(2, "February"),
    _MonthItem(3, "March"),
    _MonthItem(4, "April"),
    _MonthItem(5, "May"),
    _MonthItem(6, "June"),
    _MonthItem(7, "July"),
    _MonthItem(8, "August"),
    _MonthItem(9, "September"),
    _MonthItem(10, "October"),
    _MonthItem(11, "November"),
    _MonthItem(12, "December"),
  ];

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadData();
    });
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final provider = context.read<ReportProvider>();

      final result = await provider.getMusicOverview(
        mode: _isYearMode ? "year" : "month",
        userId: Session.userId!,
        year: _selectedYear,
        month: _isYearMode ? null : _selectedMonth,
        take: 4,
      );

      setState(() {
        _data = result;
      });
    } catch (e) {
      setState(() {
        _error = e.toString();
      });
    } finally {
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> _changeMode(bool isYearMode) async {
    setState(() {
      _isYearMode = isYearMode;
    });
    await _loadData();
  }

  Future<void> _changeYear(int year) async {
    setState(() {
      _selectedYear = year;
    });
    await _loadData();
  }

  Future<void> _changeMonth(int month) async {
    setState(() {
      _selectedMonth = month;
    });
    await _loadData();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _GlobalAnalyticsFilter(
                  isYearMode: _isYearMode,
                  selectedYear: _selectedYear,
                  selectedMonth: _selectedMonth,
                  years: _years,
                  months: _months,
                  onModeChanged: _changeMode,
                  onYearChanged: _changeYear,
                  onMonthChanged: _changeMonth,
                ),
                const SizedBox(height: 22),
                Expanded(
                  child: _buildBody(),
                ),
              ],
            ),
          ),
          const SizedBox(width: 18),
          _TrendingGenresCard(
            isLoading: _isLoading,
            genres: _data?.trendingGenres ?? [],
          ),
        ],
      ),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(),
      );
    }

    if (_error != null) {
      return Center(
        child: Text(
          _error!,
          style: const TextStyle(
            color: Colors.red,
            fontSize: 14,
          ),
        ),
      );
    }

    if (_data == null) {
      return const Center(
        child: Text(
          "No data available.",
          style: TextStyle(
            fontSize: 14,
            color: MusicOverContent.subTextColor,
          ),
        ),
      );
    }

    return SingleChildScrollView(
      child: Column(
        children: [
          _AnalyticsSection(
            title: "Most played",
            albums: _data!.mostPlayedAlbums,
            songs: _data!.mostPlayedSongs,
            artists: _data!.mostPlayedArtists,
          ),
          const SizedBox(height: 24),
          _AnalyticsSection(
            title: "Least played",
            albums: _data!.leastPlayedAlbums,
            songs: _data!.leastPlayedSongs,
            artists: _data!.leastPlayedArtists,
          ),
        ],
      ),
    );
  }
}

class _GlobalAnalyticsFilter extends StatelessWidget {
  final bool isYearMode;
  final int selectedYear;
  final int selectedMonth;
  final List<int> years;
  final List<_MonthItem> months;
  final Future<void> Function(bool) onModeChanged;
  final Future<void> Function(int) onYearChanged;
  final Future<void> Function(int) onMonthChanged;

  const _GlobalAnalyticsFilter({
    required this.isYearMode,
    required this.selectedYear,
    required this.selectedMonth,
    required this.years,
    required this.months,
    required this.onModeChanged,
    required this.onYearChanged,
    required this.onMonthChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: MusicOverContent.cardColor,
        border: Border.all(color: MusicOverContent.borderColor),
      ),
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: _SwitchHeader(
                  title: "Year",
                  active: isYearMode,
                  onTap: () => onModeChanged(true),
                ),
              ),
              Expanded(
                child: _SwitchHeader(
                  title: "Month",
                  active: !isYearMode,
                  onTap: () => onModeChanged(false),
                ),
              ),
            ],
          ),
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 14),
            decoration: const BoxDecoration(
              border: Border(
                bottom: BorderSide(color: MusicOverContent.borderColor),
              ),
            ),
            child: Wrap(
              spacing: 12,
              runSpacing: 10,
              crossAxisAlignment: WrapCrossAlignment.center,
              children: [
                const Text(
                  "Show analytics for:",
                  style: TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                    color: MusicOverContent.subTextColor,
                  ),
                ),
                _SelectionBox<int>(
                  label: "Year",
                  value: selectedYear,
                  items: years,
                  itemLabel: (year) => year.toString(),
                  onChanged: onYearChanged,
                  width: 120,
                ),
                if (!isYearMode)
                  _SelectionBox<int>(
                    label: "Month",
                    value: selectedMonth,
                    items: months.map((e) => e.value).toList(),
                    itemLabel: (month) =>
                        months.firstWhere((m) => m.value == month).label,
                    onChanged: onMonthChanged,
                    width: 170,
                  ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _AnalyticsSection extends StatelessWidget {
  final String title;
  final List<MusicStatItemResponse> albums;
  final List<MusicStatItemResponse> songs;
  final List<MusicStatItemResponse> artists;

  const _AnalyticsSection({
    required this.title,
    required this.albums,
    required this.songs,
    required this.artists,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: const TextStyle(
            fontSize: 31,
            fontWeight: FontWeight.w800,
            color: MusicOverContent.textColor,
          ),
        ),
        const SizedBox(height: 8),
        Container(
          decoration: BoxDecoration(
            color: MusicOverContent.cardColor,
            border: Border.all(color: MusicOverContent.borderColor),
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 16),
            child: Row(
              children: [
                Expanded(
                  child: _MiniChartCard(
                    title: "Top albums",
                    items: albums,
                  ),
                ),
                const SizedBox(width: 28),
                Expanded(
                  child: _MiniChartCard(
                    title: "Top songs",
                    items: songs,
                  ),
                ),
                const SizedBox(width: 28),
                Expanded(
                  child: _MiniChartCard(
                    title: "Top artists",
                    items: artists,
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}

class _MiniChartCard extends StatelessWidget {
  final String title;
  final List<MusicStatItemResponse> items;

  const _MiniChartCard({
    required this.title,
    required this.items,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 220,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicOverContent.borderColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: const TextStyle(
              fontSize: 13,
              color: MusicOverContent.textColor,
              fontWeight: FontWeight.w700,
            ),
          ),
          const SizedBox(height: 14),
          Expanded(
            child: items.isEmpty
                ? const Center(
                    child: Text(
                      "No data",
                      style: TextStyle(
                        color: MusicOverContent.subTextColor,
                        fontSize: 13,
                      ),
                    ),
                  )
                : ListView.separated(
                    itemCount: items.length,
                    separatorBuilder: (_, __) => const SizedBox(height: 10),
                    itemBuilder: (context, index) {
                      final item = items[index];
                      return _MusicStatRow(item: item);
                    },
                  ),
          ),
        ],
      ),
    );
  }
}

class _MusicStatRow extends StatelessWidget {
  final MusicStatItemResponse item;

  const _MusicStatRow({
    required this.item,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 42,
          height: 42,
          decoration: BoxDecoration(
            color: const Color(0xFFE7E7EA),
            borderRadius: BorderRadius.circular(8),
            image: item.imageUrl != null && item.imageUrl!.isNotEmpty
                ? DecorationImage(
                    image: NetworkImage(item.imageUrl!),
                    fit: BoxFit.cover,
                  )
                : null,
          ),
        ),
        const SizedBox(width: 10),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                item.title,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: const TextStyle(
                  fontSize: 13,
                  color: MusicOverContent.textColor,
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                "${item.playCount} plays",
                style: const TextStyle(
                  fontSize: 12,
                  color: MusicOverContent.subTextColor,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _TrendingGenresCard extends StatelessWidget {
  final bool isLoading;
  final List<GenreStatItemResponse> genres;

  const _TrendingGenresCard({
    required this.isLoading,
    required this.genres,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 220,
      height: 300,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicOverContent.borderColor),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            "Trending genres",
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w700,
              color: MusicOverContent.textColor,
            ),
          ),
          const SizedBox(height: 2),
          const Text(
            "based on listening history",
            style: TextStyle(
              fontSize: 12,
              color: MusicOverContent.subTextColor,
            ),
          ),
          const SizedBox(height: 14),
          if (isLoading)
            const Center(
              child: Padding(
                padding: EdgeInsets.only(top: 20),
                child: CircularProgressIndicator(),
              ),
            )
          else if (genres.isEmpty)
            const Text(
              "No genre data.",
              style: TextStyle(
                fontSize: 13,
                color: MusicOverContent.subTextColor,
              ),
            )
          else
            ...genres.map(
              (g) => Padding(
                padding: const EdgeInsets.only(bottom: 10),
                child: _GenreItem(
                  title: g.genre,
                  count: g.playCount,
                ),
              ),
            ),
        ],
      ),
    );
  }
}

class _GenreItem extends StatelessWidget {
  final String title;
  final int count;

  const _GenreItem({
    required this.title,
    required this.count,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          width: 8,
          height: 8,
          decoration: const BoxDecoration(
            color: MusicOverContent.primaryColor,
            shape: BoxShape.circle,
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: Text(
            title,
            style: const TextStyle(
              fontSize: 13,
              color: MusicOverContent.textColor,
            ),
          ),
        ),
        Text(
          "$count",
          style: const TextStyle(
            fontSize: 12,
            color: MusicOverContent.subTextColor,
            fontWeight: FontWeight.w600,
          ),
        ),
      ],
    );
  }
}

class _SelectionBox<T> extends StatelessWidget {
  final String label;
  final T value;
  final List<T> items;
  final String Function(T item) itemLabel;
  final Future<void> Function(T value) onChanged;
  final double width;

  const _SelectionBox({
    required this.label,
    required this.value,
    required this.items,
    required this.itemLabel,
    required this.onChanged,
    required this.width,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: width,
      height: 42,
      padding: const EdgeInsets.symmetric(horizontal: 12),
      decoration: BoxDecoration(
        color: Colors.white,
        border: Border.all(color: MusicOverContent.borderColor),
        borderRadius: BorderRadius.circular(8),
      ),
      child: DropdownButtonHideUnderline(
        child: DropdownButton<T>(
          value: value,
          isExpanded: true,
          icon: const Icon(
            Icons.keyboard_arrow_down_rounded,
            color: MusicOverContent.subTextColor,
          ),
          style: const TextStyle(
            fontSize: 13,
            color: MusicOverContent.textColor,
            fontWeight: FontWeight.w500,
          ),
          dropdownColor: Colors.white,
          onChanged: (newValue) async {
            if (newValue != null) {
              await onChanged(newValue);
            }
          },
          items: items.map((item) {
            return DropdownMenuItem<T>(
              value: item,
              child: Row(
                children: [
                  Text(
                    "$label:",
                    style: const TextStyle(
                      fontSize: 12,
                      color: MusicOverContent.subTextColor,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(width: 6),
                  Expanded(
                    child: Text(
                      itemLabel(item),
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        fontSize: 13,
                        color: MusicOverContent.textColor,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                ],
              ),
            );
          }).toList(),
        ),
      ),
    );
  }
}

class _SwitchHeader extends StatelessWidget {
  final String title;
  final bool active;
  final VoidCallback onTap;

  const _SwitchHeader({
    required this.title,
    required this.active,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      child: Container(
        height: 34,
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: active ? Colors.white : const Color(0xFFF2F2F4),
          border: const Border(
            bottom: BorderSide(color: MusicOverContent.borderColor),
          ),
        ),
        child: Stack(
          children: [
            if (active)
              const Positioned(
                top: 0,
                left: 0,
                right: 0,
                child: SizedBox(
                  height: 2,
                  child: ColoredBox(color: MusicOverContent.primaryColor),
                ),
              ),
            Center(
              child: Text(
                title,
                style: TextStyle(
                  fontSize: 13,
                  fontWeight: active ? FontWeight.w700 : FontWeight.w500,
                  color: active
                      ? MusicOverContent.primaryColor
                      : MusicOverContent.textColor,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _MonthItem {
  final int value;
  final String label;

  const _MonthItem(this.value, this.label);
}